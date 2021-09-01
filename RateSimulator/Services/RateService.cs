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
            var precioOneLuzTask = PrecioOneLuz(pathFiles);
            var precioOneLuz3PeriodosTask = PrecioOneLuz3Periodos(pathFiles);
            //var precioOneLuzPeriodosTask = ;

            await Task.WhenAll(precioOneLuzTask, precioOneLuz3PeriodosTask);

            //var precioOneLuz = ;
            //var precioOneLuzPeriodos = ;
            summaryByRate.Add("oneluz", precioOneLuzTask.Result);
            summaryByRate.Add("oneluz3periodos", precioOneLuz3PeriodosTask.Result);
            return summaryByRate;
        }

        private async Task<double> PrecioOneLuz(List<string> files)
        {
            var processFileTasks = new List<Task<double>>(files.Count);
            foreach (var file in files)
            {
                processFileTasks.Add(ProcessFileAsync(file, new OneLuzRate()));
                //total += await ProcessFileAsync(file, new OneLuzRate());

            }
            // await when all processFile finish
            await Task.WhenAll(processFileTasks);

            double total = 0;
            foreach (var task in processFileTasks)
            {
                total += task.Result;
            }

            return total;
        }

        private async Task<double> PrecioOneLuz3Periodos(List<string> files)
        {
            double total = 0;
            foreach (var file in files)
            {
                total += await ProcessFileAsync(file, new OneLuz3PeriodosRate());

            }
            // await when all processFile finish


            return total;
        }

        private async Task<double> ProcessFileAsync(string filePath, IRate rate)
        {
            using var reader = new StreamReader(filePath);
            // se pot canviar per un factory
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<ConsumptionDetailMap>();

            for (int i = 0; i <= 5; i++)
            {
                await csv.ReadAsync();
            }
            csv.ReadHeader();

            var line = new ConsumptionDetailLine();
            var records = csv.EnumerateRecordsAsync(line);

            double total = 0;
            try
            {
                await foreach (var consumptionLine in records)
                {
                    ConsumptionDetail consumptionDetail = consumptionLine.GetConsumptionDetail();
                    consumptionDetail.FranjaHoraria = config.GetFranja(consumptionDetail.Start);
                    total += rate.CalculateCost(consumptionDetail);
                }
            }
            catch (Exception ex)
            {

                //Console.WriteLine(ex.Message);
                Console.WriteLine($"ha acabat total {total}");

            }

            return total;
        }
    }
}
