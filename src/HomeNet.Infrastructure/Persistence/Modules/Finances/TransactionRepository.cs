using HomeNet.Core.Common;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Extensions;
using SqlKata;
using SqlKata.Execution;

namespace HomeNet.Infrastructure.Persistence.Modules.Finances;

public sealed class TransactionRepository : SqlKataRepository, ITransactionRepository
{
    private static readonly string TableName = "transactions";

    public TransactionRepository(QueryFactory db)
        : base(db)
    {
    }

    public async Task<IReadOnlyList<Expense>> GetAllExpensesAsync(
        CancellationToken cancellationToken = default)
    {
        var query = new Query(TableName)
            .Where("type", TransactionType.Expense);
        
        var transactionEntities = await GetListAsync<TransactionEntity>(query, cancellationToken);
        return transactionEntities
            .Select(e => e.ToExpense())
            .ToList();
    }

    public async Task<IReadOnlyList<Income>> GetAllIncomesAsync(
        CancellationToken cancellationToken = default)
    {
        var query = new Query(TableName)
            .Where("type", TransactionType.Income);
        
        var transactionEntities = await GetListAsync<TransactionEntity>(query, cancellationToken);
        return transactionEntities
            .Select(e => e.ToIncome())
            .ToList();
    }

    public async Task<Result> AddExpenseAsync(
        Expense expense, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new Query(TableName)
                .AsInsert(expense.ToEntity());
            
            var rows = await ExecuteAsync(query, cancellationToken);

            return rows > 0
                ? Result.Success()
                : Result.Failure("Failed to insert expense into database.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while adding the expense: {ex.Message}");
        }
    }

    public async Task<Result> AddIncomeAsync(
        Income income, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new Query(TableName)
                .AsInsert(income.ToEntity());
            
            var rows = await ExecuteAsync(query, cancellationToken);

            return rows > 0
                ? Result.Success()
                : Result.Failure("Failed to insert income into database.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while adding the income: {ex.Message}");
        }
    }
}
