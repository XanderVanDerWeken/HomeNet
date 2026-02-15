using HomeNet.Core.Common;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Extensions;
using SqlKata;

namespace HomeNet.Infrastructure.Persistence.Modules.Finances;

public sealed class FixedCostRepository : SqlKataRepository, IFixedCostRepository
{
    private static readonly string TableName = "finances.fixed_costs";
    private static readonly string IntervalTableName = "finances.fixed_cost_intervals";

    public FixedCostRepository(PostgresQueryFactory db)
        : base(db)
    {
    }

    public async Task<IReadOnlyList<FixedCost>> GetAllFixedCostsAsync(
        CancellationToken cancellationToken = default)
    {
        var query = new Query(TableName);

        var entities = await GetMultipleAsync<FixedCostEntity>(
            query, 
            cancellationToken);
        
        var tasks = entities
            .Select(e => GetFixedCostAsync(e, cancellationToken));
        
        return (await Task.WhenAll(tasks)).ToList();
    }

    public async Task<FixedCost?> GetFixedCostByIdAsync(
        int id, 
        CancellationToken cancellationToken = default)
    {
        var query = new Query(TableName)
            .Where("id", id);
        
        var entity = await FirstOrDefaultAsync<FixedCostEntity>(
            query, 
            cancellationToken);
        
        if (entity == null)
        {
            return null;
        }

        return await GetFixedCostAsync(entity, cancellationToken);
    }

    public Task<Result> AddFixedCostAsync(
        FixedCost fixedCost, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result> UpdateFixedCostAsync(
        FixedCost fixedCost, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result> RemoveFixedCostAsync(
        int id, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private async Task<FixedCost> GetFixedCostAsync(FixedCostEntity entity, CancellationToken cancellationToken)
    {
        var intervals = await GetIntervalsAsync(entity.Id, cancellationToken);  
        return new FixedCost
        {
            Id = entity.Id,
            Name = entity.Name,
            Intervals = intervals,
        };
    }

    private async Task<List<FixedCostInterval>> GetIntervalsAsync(
        int fixedCostId, 
        CancellationToken cancellationToken = default)
    {
        var query = new Query(IntervalTableName)
            .Where("fixed_cost_id", fixedCostId);
        
        var entities = await GetMultipleAsync<FixedCostIntervalEntity>(
            query, 
            cancellationToken);
        
        return entities.Select(e => e.ToFixedCostInterval()).ToList();
    }
}
