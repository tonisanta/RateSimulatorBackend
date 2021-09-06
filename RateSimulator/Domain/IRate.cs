using RateSimulator.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateSimulator.Domain
{
    public interface IRate
    {
        Task<ConsumptionSummary> ProcessFile(IAsyncEnumerable<ConsumptionDetailLine> consumptionDetailLines);
    }
}
