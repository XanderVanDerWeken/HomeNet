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

    public Task<IReadOnlyList<FixedCostWithVersion>> GetAllFixedCostsWithVersionsInMonthAsync(
        int year, 
        int month, 
        CancellationToken cancellationToken = default);
    
    public Task<IReadOnlyList<FixedCostTransaction>> GetFixedCostTransactionWithPeriodAsync(
        DateOnly period,
        CancellationToken cancellationToken = default);

    public Task<Result> AddFixedCostWithVersionAsync(
        FixedCost fixedCost, 
        FixedCostVersion fixedCostVersion, 
        CancellationToken cancellationToken = default);

    public Task<Result> CreateNewVersionAsync(
        int fixedCostId, 
        Money newAmount, 
        DateOnly validFrom, 
        CancellationToken cancellationToken = default);

    public Task<Result> DeactivateLastVersionAsync(
        int fixedCostId, 
        DateOnly validTo, 
        CancellationToken cancellationToken = default);
}
