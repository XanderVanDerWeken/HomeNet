using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;
using SqlKata;
using SqlKata.Execution;

namespace HomeNet.Infrastructure.Persistence.Modules.Finances;

public sealed class TransactionRepository : SqlKataRepository, ITransactionRepository
{
    public TransactionRepository(QueryFactory db)
        : base(db)
    {
    }

    public async Task<IReadOnlyList<Expense>> GetAllExpensesAsync(
        CancellationToken cancellationToken = default)
    {
        var query = new Query("transactions")
            .Where("type", "Expense");
        
        var transactionEntities = await GetListAsync<TransactionEntity>(query, cancellationToken);
        return transactionEntities
            .Select(e => new Expense
            {
                Id = e.Id,
                Amount = e.Amount,
                Date = e.Date,
                Category = e.Category,
                Store = e.Store!,
            }).ToList();
    }

    public async Task<IReadOnlyList<Income>> GetAllIncomesAsync(
        CancellationToken cancellationToken = default)
    {
        var query = new Query("transactions")
            .Where("type", "Income");
        
        var transactionEntities = await GetListAsync<TransactionEntity>(query, cancellationToken);
        return transactionEntities
            .Select(e => new Income
            {
                Id = e.Id,
                Amount = e.Amount,
                Date = e.Date,
                Category = e.Category,
                Source = e.IncomeSource!,
            }).ToList();
    }
}
