using System.Collections.Generic;

namespace RateSimulator.Domain
{
    public class ConsumptionSummary
    {
        public double Cost { get; set; }
        public Dictionary<string, double> ConsumptionBreakdown { get; set; }

        public ConsumptionSummary()
        {
            ConsumptionBreakdown = new Dictionary<string, double>();
        }

        public void Add(ConsumptionSummary other)
        {
            Cost += other.Cost;
            foreach (var kvp in other.ConsumptionBreakdown)
            {
                AddConsumption(kvp.Key, kvp.Value);
            }
        }

        public void AddConsumption(string key, double consumption)
        {
            if (!ConsumptionBreakdown.TryGetValue(key, out var _))
            {
                ConsumptionBreakdown[key] = 0;
            }

            ConsumptionBreakdown[key] += consumption;
        }
    }
}
