using CsvHelper.Configuration;

namespace RateSimulator.Infrastructure
{
    public class ConsumptionDetailMap : ClassMap<ConsumptionDetailLine>
    {
        // to read the csv header
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
