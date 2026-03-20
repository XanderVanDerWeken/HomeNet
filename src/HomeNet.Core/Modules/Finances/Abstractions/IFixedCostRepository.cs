using HomeNet.Core.Common;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Abstractions;

public interface IFixedCostRepository
{
    public Task<IReadOnlyList<FixedCost>> GetAllFixedCostsAsync(
        bool includeInactive = false,
        CancellationToken cancellationToken = default);

    public Task<Result<IReadOnlyList<FixedCostVersion>>> GetFixedCostHistoryAsync(
        int fixedCostId, 
        CancellationToken cancellationToken = default);
}
