using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateSimulator.Domain
{
    public interface IRate
    {
        double CalculateCost(ConsumptionDetail consumptionDetail);

        //Dictionary<string, float> GetDetail(); // torna llano -- 234.5 kw 
    }
}
