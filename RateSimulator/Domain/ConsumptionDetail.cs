using System;

namespace RateSimulator.Domain
{
    public class ConsumptionDetail
    {
        public DateTime Date { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public uint Consumption { get; set; }
        public FranjaHoraria FranjaHoraria { get; set; }
    }
}
