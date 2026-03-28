using System.Data;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Extensions;
using Microsoft.Extensions.Logging;
using SqlKata;

namespace HomeNet.Infrastructure.Persistence.Modules.Finances;

public sealed class FixedCostTransactionRepository : SqlKataRepository, IFixedCostTransactionRepository
{
    private static readonly string TransactionsTableName = "finances.transactions";
    private static readonly string FixedCostTransactionsTableName = "finances.fixed_cost_transactions";

    private readonly ILogger _logger;
    private readonly IDbConnection _dbConnection;

    public FixedCostTransactionRepository(
        ILogger<FixedCostTransactionRepository> logger,
        PostgresQueryFactory db)
        : base(db)
    {
        _logger = logger;
        _dbConnection = db.Connection;
    }

    public async Task<IReadOnlyList<FixedCostTransaction>> GetFixedCostTransactionWithPeriodAsync(
        DateOnly period,
        CancellationToken cancellationToken = default)
    {
        var query = new Query(FixedCostTransactionsTableName)
            .Where("period", period);
        
        var entities = await GetMultipleAsync<FixedCostTransactionEntity>(
            query, 
            cancellationToken: cancellationToken);
        
        return entities
            .Select(e => e.ToFixedCostTransaction())
            .ToList();
    }

    public async Task AddFixedCostTransactionAsync(
        FixedCostTransaction fixedCostTransaction,
        IDbTransaction dbTransaction,
        CancellationToken cancellationToken = default)
    {
        var query = new Query(FixedCostTransactionsTableName).AsInsert(new
        {
            fixed_cost_id = fixedCostTransaction.FixedCostId,
            fixed_cost_version_id = fixedCostTransaction.FixedCostVersionId,
            transaction_id = fixedCostTransaction.TransactionId,
            period = fixedCostTransaction.Period,
        });

        var fixedCostTransactionId = await InsertAndReturnIdAsync(query, dbTransaction);
        fixedCostTransaction.Id = fixedCostTransactionId;
    }

    public async Task AddTransactionAsync(
        Transaction transaction, 
        IDbTransaction dbTransaction, 
        CancellationToken cancellationToken = default)
    {
        var query = new Query(TransactionsTableName).AsInsert(new
        {
            category_id = transaction.CategoryId,
            amount = transaction.Amount.Amount,
            date = transaction.Date,
            type = transaction.Type.ToString(),
        });

        var transactionId = await InsertAndReturnIdAsync(query, dbTransaction);
        transaction.Id = transactionId;
    }
}
