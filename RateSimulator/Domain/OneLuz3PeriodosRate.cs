using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateSimulator.Domain
{
    public class OneLuz3PeriodosRate : IRate
    {
        public double PricePerKwHPunta { get; set; } = 0.303723;
        public double PricePerKwHLlano { get; set; } = 0.189849;
        public double PricePerKwHValle { get; set; } = 0.141266;

        public double CalculateCost(ConsumptionDetail consumptionDetail)
        {
            var ratePeriod = consumptionDetail.FranjaHoraria.Nombre;
            double price;

            switch (ratePeriod)
            {
                case "punta":
                    price = PricePerKwHPunta;
                    break;
                case "llano":
                    price = PricePerKwHLlano;
                    break;
                case "valle":
                    price = PricePerKwHValle;
                    break;
                default: return -1;
            }

            bool isWeekend = consumptionDetail.Date.DayOfWeek == DayOfWeek.Saturday || consumptionDetail.Date.DayOfWeek == DayOfWeek.Sunday;
            if (isWeekend)
            {
                price = PricePerKwHValle;
            }

            return price * (consumptionDetail.Consumption / 1000.0);
        }
    }
}
