using RateSimulator.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

        private List<FranjaHoraria> _intervalos;

        public void ParseIntervals()
        {
            _intervalos = new List<FranjaHoraria>();

            foreach (string s in InicioValle)
            {
                TimeSpan inicio = TimeSpan.Parse(s);
                FranjaHoraria franja = new FranjaHoraria("valle", inicio);
                _intervalos.Add(franja);
            }

            foreach (string s in InicioLlano)
            {
                TimeSpan inicio = TimeSpan.Parse(s);
                FranjaHoraria franja = new FranjaHoraria("llano", inicio);
                _intervalos.Add(franja);
            }

            foreach (string s in InicioPunta)
            {
                TimeSpan inicio = TimeSpan.Parse(s);
                FranjaHoraria franja = new FranjaHoraria("punta", inicio);
                _intervalos.Add(franja);
            }

            _intervalos.Sort();
        }

        public FranjaHoraria GetFranja(TimeSpan time)
        {
            FranjaHoraria franjaActual = new FranjaHoraria("", time);
            int index = _intervalos.BinarySearch(franjaActual);
            int actual;

            if (index >= 0)
            {
                actual = index;
                int siguiente = (index + 1) % _intervalos.Count;
                franjaActual = _intervalos.ElementAt(index);
                franjaActual.Final = _intervalos.ElementAt(siguiente).Inicio;
            }
            else
            {
                actual = (~index - 1) % _intervalos.Count;
                int next = (actual + 1) % _intervalos.Count;

                franjaActual = _intervalos.ElementAt(actual);
                franjaActual.Final = _intervalos.ElementAt(next).Inicio;
            }

            return franjaActual;
        }
    }
}
