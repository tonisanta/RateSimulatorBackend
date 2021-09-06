using System;

namespace RateSimulator.Domain
{
    public class Period : IComparable
    {
        public string Name { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }

        public Period(string name, TimeSpan start)
        {
            this.Name = name;
            this.Start = start;
        }

        public int CompareTo(object obj)
        {
            Period franja = obj as Period;
            if (franja == null) return 1;
            return this.Start.CompareTo(franja.Start);
        }
    }
}
