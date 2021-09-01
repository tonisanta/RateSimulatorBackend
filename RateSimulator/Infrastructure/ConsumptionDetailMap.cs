using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateSimulator.Infrastructure
{
    public class ConsumptionDetailMap : ClassMap<ConsumptionDetailLine>
    {
        public ConsumptionDetailMap()
        {
            Map(m => m.Date).Name("Fecha");
            Map(m => m.Hour).Name("Hora");
            Map(m => m.Consumption).Name("Consumo (Wh)");
            Map(m => m.PricePerkWh).Name("Precio (€/kWh)");
            Map(m => m.PricePerHour).Name("Coste por hora (€)");
        }
    }
}
