using RateSimulator.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateSimulator.Infrastructure
{
    public class ConsumptionDetailLine
    {
        public DateTime Date { get; set; }
        public string Hour { get; set; }
        public uint Consumption { get; set; }
        public uint PricePerkWh { get; set; }
        public uint PricePerHour { get; set; }

        public ConsumptionDetail GetConsumptionDetail()
        {
            var consDetail = new ConsumptionDetail
            {
                Consumption = this.Consumption,
                Date = this.Date,

            };
            ParsePeriod(consDetail);
            return consDetail;
        }

        private void ParsePeriod(ConsumptionDetail consumptionDetail)
        {
            string[] subs = Hour.Split('-');
            TimeSpan start = TimeSpan.Parse(subs[0]);
            TimeSpan end = TimeSpan.Parse(subs[1]);
            consumptionDetail.Start = start;
            consumptionDetail.End = end;
        }
    }
}
