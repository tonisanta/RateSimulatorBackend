using RateSimulator.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateSimulator.Domain
{
    public class OneLuz3PeriodosRate : IRate
    {
        private readonly ConfigurationPeriods config;
        private readonly PriceConfiguration priceConfig;
        private ConsumptionSummary Summary;

        public OneLuz3PeriodosRate(ConfigurationPeriods config, PriceConfiguration priceConfig)
        {
            this.config = config;
            this.priceConfig = priceConfig;
            Summary = new ConsumptionSummary
            {
                ConsumptionBreakdown = new Dictionary<string, double>(3)
                {
                    { "llano", 0 },
                    { "punta", 0 },
                    { "valle", 0 }
                }
            };
        }

        public async Task<ConsumptionSummary> ProcessFile(IAsyncEnumerable<ConsumptionDetailLine> consumptionDetailLines)
        {
            await foreach (var consumptionLine in consumptionDetailLines)
            {
                ConsumptionDetail consumptionDetail = consumptionLine.GetConsumptionDetail();
                consumptionDetail.Period = config.GetClosestPeriod(consumptionDetail.Start);
                StoreConsumption(consumptionDetail);
            }
            CalculateCost();
            return Summary;
        }

        private void StoreConsumption(ConsumptionDetail consumptionDetail)
        {
            var dayOfWeek = consumptionDetail.Date.DayOfWeek;
            bool isWeekend = dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday;
            var ratePeriod = isWeekend ? "valle" : consumptionDetail.Period.Name; // during the weekend is cheaper
            Summary.AddConsumption(ratePeriod, consumptionDetail.Consumption);
        }

        private void CalculateCost()
        {
            Summary.Cost += priceConfig.PricePerKwHLlano * (Summary.ConsumptionBreakdown["llano"] / 1000.0);
            Summary.Cost += priceConfig.PricePerKwHPunta * (Summary.ConsumptionBreakdown["punta"] / 1000.0);
            Summary.Cost += priceConfig.PricePerKwHValle * (Summary.ConsumptionBreakdown["valle"] / 1000.0);
        }
    }
}
