using System.Data;
using HomeNet.Core.Common;
using HomeNet.Core.Common.Errors;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Extensions;
using Microsoft.Extensions.Logging;
using SqlKata;

namespace HomeNet.Infrastructure.Persistence.Modules.Finances;

public sealed class FixedCostRepository : SqlKataRepository, IFixedCostRepository
{
    private static readonly string FixedCostTableName = "finances.fixed_costs";
    private static readonly string FixedCostVersionTableName = "finances.fixed_cost_versions";

    private readonly ILogger _logger;

    public FixedCostRepository(
        ILogger<FixedCostRepository> logger,
        PostgresQueryFactory db)
        : base(db)
    {
        _logger = logger;
    }

    public async Task<IReadOnlyList<FixedCost>> GetAllFixedCostsAsync(
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        // TODO: Add Include
        var query = new Query($"FixedCostTableName as fc");

        var entities = await GetMultipleAsync<FixedCostEntity>(
            query,
            cancellationToken: cancellationToken);
        
        return entities
            .Select(e => e.ToFixedCost())
            .ToList();
    }

    public async Task<Result<IReadOnlyList<FixedCostVersion>>> GetFixedCostHistoryAsync(
        int fixedCostId, 
        CancellationToken cancellationToken = default)
    {
        var query = new Query(FixedCostVersionTableName)
            .Where("fixed_cost_id", fixedCostId);
        
        var entities = await GetMultipleAsync<FixedCostVersionEntity>(
            query, 
            cancellationToken: cancellationToken);
        
        return Result<IReadOnlyList<FixedCostVersion>>.Success(entities
            .Select(e => e.ToFixedCostVersion())
            .ToList());
    }

    public async Task<IReadOnlyList<FixedCostWithVersion>> GetAllFixedCostsWithVersionsInMonthAsync(
        int year, 
        int month, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> AddFixedCostWithVersionAsync(
        FixedCost fixedCost, 
        FixedCostVersion fixedCostVersion, 
        CancellationToken cancellationToken = default)

    {
        using var tx = _db.Connection.BeginTransaction();

        try
        {
            var fixedCostId = await InsertFixedCostAsync(fixedCost, tx);
            
            var fixedCostVersionId = await InsertFixedCostVersionAsync(
                fixedCostId, 
                fixedCostVersion.Amount, 
                fixedCostVersion.ValidFrom, 
                fixedCostVersion.ValidTo, 
                tx);

            tx.Commit();

            fixedCost.Id = fixedCostId;
            fixedCostVersion.Id =fixedCostVersionId;
            fixedCostVersion.FixedCostId = fixedCostId;

            return Result.Success();
        }
        catch (Exception ex)
        {
            tx.Rollback();
            return new DatabaseError(FixedCostTableName, ex).ToFailure();
        }
    }

    public async Task<Result> CreateNewVersionAsync(
        int fixedCostId, 
        Money newAmount, 
        DateOnly validFrom, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Inserting new fixed cost version");
            var fixedCostVersionId = await InsertFixedCostVersionAsync(
                fixedCostId, 
                newAmount, 
                validFrom);
            
            _logger.LogInformation(
                "Fixed Cost Version inserted successfully with ID: {FixedCostVersionId}",
                fixedCostVersionId);
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Error occurred while adding fixed cost version for fixed cost with id: {Id}",
                fixedCostId);
            return new DatabaseError(FixedCostVersionTableName, ex).ToFailure();
        }
    }

    public async Task<Result> DeactivateLastVersionAsync(
        int fixedCostId, 
        DateOnly validTo, 
        CancellationToken cancellationToken = default)
    {
        var entityToDeactivate = await GetLastVersionAsync(fixedCostId, cancellationToken);

        if (entityToDeactivate is null)
        {
            return new NotFoundError("FixedCostVersion", fixedCostId)
                .ToFailure();
        }

        if (entityToDeactivate.ValidTo is not null && entityToDeactivate.ValidTo < validTo)
        {
            return new InvalidOperationError(
                "Cannot deactivate a version that is already inactive.")
                .ToFailure();
        }

        await UpdateVersion(entityToDeactivate.Id, validTo, cancellationToken);
        return Result.Success();
    }

    private Task<int> InsertFixedCostAsync(
        FixedCost fixedCost,
        IDbTransaction? tx)
    {
        var query = new Query(FixedCostTableName)
            .AsInsert(new
            {
                category_id = fixedCost.CategoryId,
                name = fixedCost.Name,
                day_of_month = fixedCost.DayOfMonth,
            });
        
        return InsertAndReturnIdAsync(query, tx);
    }

    private Task<int> InsertFixedCostVersionAsync(
        int fixedCostId,
        Money amount,
        DateOnly validFrom,
        DateOnly? validTo = null,
        IDbTransaction? tx = null)
    {
        var query = new Query(FixedCostVersionTableName)
            .AsInsert(new
            {
                fixed_cost_id = fixedCostId,
                amount = amount.Amount,
                valid_from = validFrom,
                valid_to = validTo,
            });
        
        return InsertAndReturnIdAsync(query, tx);
    }

    private Task<FixedCostVersionEntity?> GetLastVersionAsync(
        int fixedCostId, 
        CancellationToken cancellationToken = default)
    {
        var query = new Query(FixedCostVersionTableName)
            .Where("fixed_cost_id", fixedCostId)
            .OrderByDesc("valid_from")
            .Limit(1);
        
        return FirstOrDefaultAsync<FixedCostVersionEntity?>(
            query,
            cancellationToken: cancellationToken);
    }

    private async Task<Result> UpdateVersion(int fixedCostVersionId, DateOnly validTo, CancellationToken cancellationToken)
    {
        try
        {
            var query = new Query(FixedCostVersionTableName)
                .Where("id", fixedCostVersionId)
                .AsUpdate(new
                {
                    valid_to = validTo,
                });
            
            var affectedRows = await ExecuteAsync(
                query, 
                cancellationToken: cancellationToken);
            
            return affectedRows > 0
                ? Result.Success()
                : new NotFoundError("FixedCostVersion", fixedCostVersionId).ToFailure();
        }
        catch (Exception ex)
        {
            return new DatabaseError(FixedCostVersionTableName, ex).ToFailure();
        }
    }
}
