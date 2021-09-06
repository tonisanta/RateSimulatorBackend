using RateSimulator.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateSimulator.Domain
{
    public class OneLuzRate : IRate
    {
        private readonly ConfigurationPeriods config;
        private readonly PriceConfiguration priceConfig;

        public ConsumptionSummary Summary { get; set; }

        public OneLuzRate(ConfigurationPeriods config, PriceConfiguration priceConfig)
        {
            this.config = config;
            this.priceConfig = priceConfig;
            Summary = new ConsumptionSummary
            {
                ConsumptionBreakdown = new Dictionary<string, double>(1)
            };
        }

        public async Task<ConsumptionSummary> ProcessFile(IAsyncEnumerable<ConsumptionDetailLine> consumptionDetailLines)
        {
            double totalConsumption = 0;
            await foreach (var consumptionLine in consumptionDetailLines)
            {
                ConsumptionDetail consumptionDetail = consumptionLine.GetConsumptionDetail();
                consumptionDetail.FranjaHoraria = config.GetFranja(consumptionDetail.Start);
                totalConsumption += consumptionLine.Consumption;
            }

            Summary.ConsumptionBreakdown = new Dictionary<string, double>
            {
                {"unique-rate", totalConsumption}
            };
            CalculateCost();
            return Summary;
        }

        private void CalculateCost()
        {
            Summary.Cost = priceConfig.PricePerKwH * (Summary.ConsumptionBreakdown["unique-rate"] / 1000.0);
        }
    }
}
