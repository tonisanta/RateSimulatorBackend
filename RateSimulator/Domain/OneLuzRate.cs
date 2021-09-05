using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateSimulator.Domain
{
    public class OneLuzRate : IRate
    {
        public double PricePerKwH { get; set; } = 0.201117;

        public double CalculateCost(ConsumptionDetail consumptionDetail)
        {
            return PricePerKwH * (consumptionDetail.Consumption / 1000.0);
        }
    }
}
