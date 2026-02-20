using HomeNet.Core.Common;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Abstractions;

public interface IFixedCostRepository
{
    public Task<IReadOnlyList<FixedCost>> GetAllFixedCostsAsync(
        CancellationToken cancellationToken = default);

    public Task<FixedCost?> GetFixedCostByNameAsync(
        string name, 
        CancellationToken cancellationToken = default);
    
    public Task<Result> AddFixedCostAsync(
        FixedCost fixedCost, 
        CancellationToken cancellationToken = default);
    
    public Task<Result> AddFixedCostIntervalAsync(
        FixedCostInterval interval,
        string fixedCostName,
        CancellationToken cancellationToken = default);
}
