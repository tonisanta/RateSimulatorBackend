using RateSimulator.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace RateSimulator
{
    public class ConfigurationPeriods
    {
        [JsonPropertyName("valle")]
        public List<string> InicioValle { get; set; }
        [JsonPropertyName("llano")]
        public List<string> InicioLlano { get; set; }
        [JsonPropertyName("punta")]
        public List<string> InicioPunta { get; set; }

        private List<Period> periods;

        public void ParseIntervals()
        {
            periods = new List<Period>();

            foreach (string s in InicioValle)
            {
                TimeSpan start = TimeSpan.Parse(s);
                Period period = new Period("valle", start);
                periods.Add(period);
            }

            foreach (string s in InicioLlano)
            {
                TimeSpan start = TimeSpan.Parse(s);
                Period period = new Period("llano", start);
                periods.Add(period);
            }

            foreach (string s in InicioPunta)
            {
                TimeSpan start = TimeSpan.Parse(s);
                Period period = new Period("punta", start);
                periods.Add(period);
            }

            periods.Sort();
        }

        public Period GetClosestPeriod(TimeSpan time)
        {
            Period currentPeriod = new Period("", time);
            int index = periods.BinarySearch(currentPeriod);

            if (index >= 0)
            {
                int next = (index + 1) % periods.Count;
                currentPeriod = periods.ElementAt(index);
                currentPeriod.End = periods.ElementAt(next).Start;
            }
            else
            {
                int current = (~index - 1) % periods.Count;
                int next = (current + 1) % periods.Count;

                currentPeriod = periods.ElementAt(current);
                currentPeriod.End = periods.ElementAt(next).Start;
            }

            return currentPeriod;
        }
    }
}
