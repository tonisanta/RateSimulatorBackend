using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateSimulator
{
    public class PriceConfiguration
    {
        public double PricePerKwHPunta { get; set; } = 0.303723;
        public double PricePerKwHLlano { get; set; } = 0.189849;
        public double PricePerKwHValle { get; set; } = 0.141266;

        public double PricePerKwH { get; set; } = 0.201117;
    }
}
