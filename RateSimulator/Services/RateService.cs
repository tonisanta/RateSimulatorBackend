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
            var summaryByRate = new Dictionary<string, ConsumptionSummary>(2);
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
            using var reader = new StreamReader(fileStream);

            // skip header info. Would be better to be able to skip it with a seek but it has a dynamic size
            for (int i = 0; i <= 5; i++)
            {
                await reader.ReadLineAsync().ConfigureAwait(false);
            }

            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<ConsumptionDetailMap>();

            var records = GetLinesAsync(csv);
            return await rate.ProcessFile(records);
        }

        private async IAsyncEnumerable<ConsumptionDetailLine> GetLinesAsync(CsvReader csv)
        {
            while (await csv.ReadAsync())
            {
                var firstItem = csv.GetField(0);
                // to skip last line as it doesn't follow the pattern: ,Total (Wh):,xyz
                if (string.IsNullOrWhiteSpace(firstItem)) break;

                yield return csv.GetRecord<ConsumptionDetailLine>();
            }

        }
    }
}
