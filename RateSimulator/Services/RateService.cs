using CsvHelper;
using Microsoft.Extensions.Options;
using RateSimulator.Domain;
using RateSimulator.Infrastructure;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace RateSimulator.Services
{
    public class RateService
    {
        private readonly ConfigurationPeriods periodConfig;

        public RateService(IOptions<ConfigurationPeriods> config)
        {
            this.periodConfig = config.Value;
            this.periodConfig.ParseIntervals();
        }

        public async Task<Dictionary<string, ConsumptionSummary>> ProcessFilesAsync(List<string> pathFiles, PriceConfiguration priceConfig)
        {
            var summaryByRate = new Dictionary<string, ConsumptionSummary>(2); // por ahora hay 2 tarifas 
            var precioOneLuzTask = PriceForAllFiles(pathFiles, new OneLuzFactory(periodConfig, priceConfig));
            var precioOneLuz3PeriodosTask = PriceForAllFiles(pathFiles, new OneLuz3PeriodosFactory(periodConfig, priceConfig));

            await Task.WhenAll(precioOneLuzTask, precioOneLuz3PeriodosTask).ConfigureAwait(false);

            summaryByRate.Add("oneLuz", precioOneLuzTask.Result);
            summaryByRate.Add("oneLuz3Periodos", precioOneLuz3PeriodosTask.Result);
            return summaryByRate;
        }

        private async Task<ConsumptionSummary> PriceForAllFiles(List<string> files, IRateFactory rateFactory)
        {
            var processFileTasks = new List<Task<ConsumptionSummary>>(files.Count);
            foreach (var file in files)
            {
                processFileTasks.Add(ProcessFileAsync(file, rateFactory.GetInstance()));
            }

            await Task.WhenAll(processFileTasks).ConfigureAwait(false);

            var summary = new ConsumptionSummary();
            foreach (var task in processFileTasks)
            {
                var taskSummary = task.Result;
                summary.Add(taskSummary);
            }

            return summary;
        }

        private async Task<ConsumptionSummary> ProcessFileAsync(string filePath, IRate rate)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            fileStream.Seek(157, SeekOrigin.Begin); // skip initial info
            using var reader = new StreamReader(fileStream);

            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<ConsumptionDetailMap>();

            var records = csv.GetRecordsAsync<ConsumptionDetailLine>();
            return await rate.ProcessFile(records);
        }
    }
}
