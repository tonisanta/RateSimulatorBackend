using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateSimulator.Domain
{
    public class FranjaHoraria : IComparable
    {
        public string Nombre { get; set; }
        public TimeSpan Inicio { get; set; }
        public TimeSpan Final { get; set; }


        public FranjaHoraria(string nombre, TimeSpan inicio)
        {
            this.Nombre = nombre;
            this.Inicio = inicio;
        }

        public override string ToString()
        {
            return $"*{Nombre}* {Inicio} - {Final}";
        }

        public int CompareTo(object obj)
        {
            FranjaHoraria franja = obj as FranjaHoraria;
            if (franja == null) return 1;
            return this.Inicio.CompareTo(franja.Inicio);
        }
    }
}
