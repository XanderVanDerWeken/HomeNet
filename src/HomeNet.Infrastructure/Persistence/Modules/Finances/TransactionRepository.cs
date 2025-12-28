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
    
    private readonly ICategoryRepository _categoryRepository;

    public TransactionRepository(
        PostgresQueryFactory db, 
        ICategoryRepository categoryRepository)
        : base(db)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<Income>> GetAllIncomesAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var query = new Query(TableName)
            .Where("transaction_type", TransactionType.Income)
            .Where("year", year)
            .Where("month", month);
        
        var entities = await GetMultipleAsync<TransactionEntity>(
            query, 
            cancellationToken);
        
        var category = await _categoryRepository.GetCategoryByIdAsync(1, cancellationToken);

        return entities
            .Select(e => e.ToIncome(category!))
            .ToList();
    }
    
    public async Task<IReadOnlyList<Expense>> GetAllExpensesAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var query = new Query(TableName)
            .Where("transaction_type", TransactionType.Expense)
            .Where("year", year)
            .Where("month", month);
        
        var entities = await GetMultipleAsync<TransactionEntity>(
            query, 
            cancellationToken);
        
        var category = await _categoryRepository.GetCategoryByIdAsync(1, cancellationToken);

        return entities
            .Select(e => e.ToExpense(category!))
            .ToList();
    }

    public async Task<Result> AddExpenseAsync(
        Expense expense, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new Query(TableName)
                .AsInsert(new
                {
                    amount = expense.Amount.Amount,
                    year = expense.Date.Year,
                    month = expense.Date.Month,
                    transaction_type = TransactionType.Expense,
                    category_id = expense.Category.Id,
                    store = expense.Store,
                });
            
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
                .AsInsert(new
                {
                    amount = income.Amount.Amount,
                    year = income.Date.Year,
                    month = income.Date.Month,
                    transaction_type = TransactionType.Income,
                    category_id = income.Category.Id,
                    income_source = income.Source,
                });
            
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
