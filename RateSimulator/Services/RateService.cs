using CsvHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RateSimulator.Domain;
using RateSimulator.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RateSimulator.Services
{
    public class RateService
    {
        private readonly ConfigurationPeriods config;

        public RateService(IOptions<ConfigurationPeriods> config)
        {
            this.config = config.Value;
            this.config.ParseIntervals();
        }

        public async Task<Dictionary<string, double>> ProcessFilesAsync(List<string> pathFiles)
        {
            var summaryByRate = new Dictionary<string, double>(2); // por ahora hay 2 tarifas 
            var precioOneLuzTask = PriceForAllFiles(pathFiles, new OneLuzRate());
            var precioOneLuz3PeriodosTask = PriceForAllFiles(pathFiles, new OneLuz3PeriodosRate());

            await Task.WhenAll(precioOneLuzTask, precioOneLuz3PeriodosTask).ConfigureAwait(false);

            summaryByRate.Add("oneluz", precioOneLuzTask.Result);
            summaryByRate.Add("oneluz3periodos", precioOneLuz3PeriodosTask.Result);
            return summaryByRate;
        }

        private async Task<double> PriceForAllFiles(List<string> files, IRate rate)
        {
            var processFileTasks = new List<Task<double>>(files.Count);
            foreach (var file in files)
            {
                processFileTasks.Add(ProcessFileAsync(file, rate));
            }
            // await when all processFile finish
            await Task.WhenAll(processFileTasks).ConfigureAwait(false);

            double total = 0;
            foreach (var task in processFileTasks)
            {
                total += task.Result;
            }

            return total;
        }


        private async Task<double> ProcessFileAsync(string filePath, IRate rate)
        {            
            using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            fileStream.Seek(157, SeekOrigin.Begin);
            using var reader = new StreamReader(fileStream);

            // se pot canviar per un factory
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<ConsumptionDetailMap>();

            var line = new ConsumptionDetailLine();            
            var records = csv.EnumerateRecordsAsync(line);

            double total = 0;
            await foreach (var consumptionLine in records)
            {
                ConsumptionDetail consumptionDetail = consumptionLine.GetConsumptionDetail();
                consumptionDetail.FranjaHoraria = config.GetFranja(consumptionDetail.Start);
                total += rate.CalculateCost(consumptionDetail);
            }

            return total;
        }
    }
}
