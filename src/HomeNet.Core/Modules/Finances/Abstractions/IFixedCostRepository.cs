using HomeNet.Core.Common;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Abstractions;

public interface IFixedCostRepository
{
    Task<IReadOnlyList<FixedCost>> GetAllFixedCostsAsync(
        CancellationToken cancellationToken = default);

    Task<FixedCost?> GetFixedCostByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Result> AddFixedCostAsync(
        FixedCost fixedCost,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateFixedCostAsync(
        FixedCost fixedCost,
        CancellationToken cancellationToken = default);
    
    Task<Result> RemoveFixedCostAsync(
        int id,
        CancellationToken cancellationToken = default);
}
