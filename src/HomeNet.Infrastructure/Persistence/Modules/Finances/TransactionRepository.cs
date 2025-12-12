using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Extensions;
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
            .Where("type", TransactionType.Expense);
        
        var transactionEntities = await GetListAsync<TransactionEntity>(query, cancellationToken);
        return transactionEntities
            .Select(e => e.ToExpense())
            .ToList();
    }

    public async Task<IReadOnlyList<Income>> GetAllIncomesAsync(
        CancellationToken cancellationToken = default)
    {
        var query = new Query("transactions")
            .Where("type", TransactionType.Income);
        
        var transactionEntities = await GetListAsync<TransactionEntity>(query, cancellationToken);
        return transactionEntities
            .Select(e => e.ToIncome())
            .ToList();
    }
}
