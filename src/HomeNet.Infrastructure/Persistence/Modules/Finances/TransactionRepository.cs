using HomeNet.Core.Common;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Extensions;
using SqlKata;

namespace HomeNet.Infrastructure.Persistence.Modules.Finances;

public sealed class TransactionRepository : SqlKataRepository, ITransactionRepository
{
    private static readonly string TableName = "finances.transactions";

    public TransactionRepository(PostgresQueryFactory db)
        : base(db)
    {
    }

    public async Task<IReadOnlyList<Income>> GetAllIncomesAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var query = new Query(TableName)
            .Where("type", TransactionType.Income)
            .Where("year", year)
            .Where("month", month);
        
        var entities = await GetMultipleAsync<TransactionEntity>(
            query, 
            cancellationToken);
        
        return entities
            .Select(e => e.ToIncome())
            .ToList();
    }
    
    public async Task<IReadOnlyList<Expense>> GetAllExpensesAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var query = new Query(TableName)
            .Where("type", TransactionType.Expense)
            .Where("year", year)
            .Where("month", month);
        
        var entities = await GetMultipleAsync<TransactionEntity>(
            query, 
            cancellationToken);
        
        return entities
            .Select(e => e.ToExpense())
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
            
            var newExpenseId = await InsertAndReturnIdAsync(query);
            expense.Id = newExpenseId;

            return Result.Success();
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
            
            var newIncomeId = await InsertAndReturnIdAsync(query);
            income.Id = newIncomeId;

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while adding the income: {ex.Message}");
        }
    }
}
