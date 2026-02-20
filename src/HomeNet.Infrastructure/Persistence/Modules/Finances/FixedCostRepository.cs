using HomeNet.Core.Common;
using HomeNet.Core.Common.Errors;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;
using SqlKata;

namespace HomeNet.Infrastructure.Persistence.Modules.Finances;

public sealed class FixedCostRepository : SqlKataRepository, IFixedCostRepository
{
    private static readonly string FixedCostsTableName = "finances.fixed_costs";
    private static readonly string FixedCostIntervalsTableName = "finances.fixed_cost_intervals";

    public FixedCostRepository(PostgresQueryFactory db)
        : base(db)
    {
    }

    public async Task<IReadOnlyList<FixedCost>> GetAllFixedCostsAsync(
        CancellationToken cancellationToken = default)
    {
        var query = new Query(FixedCostsTableName);

        var fixedCostsEntities = await GetMultipleAsync<FixedCostEntity>(
            query, cancellationToken);
        
        if (cancellationToken.IsCancellationRequested)
        {
            return Array.Empty<FixedCost>();
        }

        List<FixedCost> fixedCosts = [];
        foreach (var fixedCostEntity in fixedCostsEntities)
        {
            fixedCosts.Add(new FixedCost
            {
                Id = fixedCostEntity.Id,
                Name = fixedCostEntity.Name,
                Intervals = (await GetFixedCostIntervalEntitiesAsync(
                    fixedCostEntity.Id, cancellationToken))
                    .Select(e => new FixedCostInterval
                    {
                        Id = e.Id,
                        Amount = new Money(e.Amount),
                        StartYear = e.StartYear,
                        StartMonth = e.StartMonth,
                        EndYear = e.EndYear,
                        EndMonth = e.EndMonth,
                    })
                    .ToList(),
            });
        }

        return fixedCosts;
    }

    public async Task<FixedCost?> GetFixedCostByNameAsync(
        string name, 
        CancellationToken cancellationToken = default)
    {
        var fixedCostEntity = await GetFixedCostEntityByNameAsync(
            name, cancellationToken);
        
        if (fixedCostEntity == null || cancellationToken.IsCancellationRequested)
        {
            return null;
        }

        var fixedCostIntervalEntities = await GetFixedCostIntervalEntitiesAsync(
            fixedCostEntity.Id, cancellationToken);
        
        return new FixedCost
        {
            Id = fixedCostEntity.Id,
            Name = fixedCostEntity.Name,
            Intervals = fixedCostIntervalEntities
                .Select(e => new FixedCostInterval
                {
                    Id = e.Id,
                    Amount = new Money(e.Amount),
                    StartYear = e.StartYear,
                    StartMonth = e.StartMonth,
                    EndYear = e.EndYear,
                    EndMonth = e.EndMonth,
                })
                .ToList(),
        };
    }

    public async Task<Result> AddFixedCostAsync(
        FixedCost fixedCost, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new Query(FixedCostsTableName)
                .AsInsert(new
                {
                    name = fixedCost.Name,
                });
            
            var newId = await InsertAndReturnIdAsync(query);
            fixedCost.Id = newId;

            return Result.Success();
        }
        catch (Exception ex)
        {
            return new DatabaseError(FixedCostsTableName, ex).ToFailure();
        }
    }

    public async Task<Result> AddFixedCostIntervalAsync(
        FixedCostInterval interval,
        string fixedCostName,
        CancellationToken cancellationToken = default)
    {
        var fixedCostEntity = await GetFixedCostByNameAsync(
            fixedCostName, cancellationToken);
        
        if (fixedCostEntity is null)
        {
            return new NotFoundError("FixedCost", fixedCostName).ToFailure();
        }

        try
        {
            var query = new Query(FixedCostIntervalsTableName)
                .AsInsert(new
                {
                    fixed_cost_id = fixedCostEntity.Id,
                    amount = interval.Amount,
                    start_year = interval.StartYear,
                    start_month = interval.StartMonth,
                    end_year = interval.EndYear,
                    end_month = interval.EndMonth,
                });
            
            var newId = await InsertAndReturnIdAsync(query);
            interval.Id = newId;

            return Result.Success();
        }
        catch (Exception ex)
        {
            return new DatabaseError(FixedCostIntervalsTableName, ex).ToFailure();
        }
    }

    private async Task<FixedCostEntity?> GetFixedCostEntityByNameAsync(
        string fixedCostName,
        CancellationToken cancellationToken = default)
    {
        var query = new Query(FixedCostsTableName)
            .Where("name", fixedCostName);
        
        return await FirstOrDefaultAsync<FixedCostEntity>(
            query, cancellationToken);
    }

    private async Task<IReadOnlyList<FixedCostIntervalEntity>> GetFixedCostIntervalEntitiesAsync(
        int fixedCostId, 
        CancellationToken cancellationToken = default)
    {
        var query = new Query(FixedCostIntervalsTableName)
            .Where("fixed_cost_id", fixedCostId);
        
        var rows = await GetMultipleAsync<FixedCostIntervalEntity>(query, cancellationToken);
        return rows.ToList();
    }
}
