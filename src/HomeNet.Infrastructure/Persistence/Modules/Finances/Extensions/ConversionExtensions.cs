using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;

namespace HomeNet.Infrastructure.Persistence.Modules.Finances.Extensions;

public static class ConversionExtensions
{
    public static TransactionEntity ToEntity(this Income income)
        => new TransactionEntity
        {
            Id = income.Id,
            Amount = income.Amount,
            Date = income.Date,
            Category = income.Category,
            Type = TransactionType.Income,
            IncomeSource = income.Source,
        };
    
    public static TransactionEntity ToEntity(this Expense expense)
        => new TransactionEntity
        {
            Id = expense.Id,
            Amount = expense.Amount,
            Date = expense.Date,
            Category = expense.Category,
            Type = TransactionType.Expense,
            Store = expense.Store,
        };
    
    public static Income ToIncome(this TransactionEntity entity)
        => new Income
        {
            Id = entity.Id,
            Amount = entity.Amount,
            Date = entity.Date,
            Category = entity.Category,
            Source = entity.IncomeSource!,
        };
    
    public static Expense ToExpense(this TransactionEntity entity)
        => new Expense
        {
            Id = entity.Id,
            Amount = entity.Amount,
            Date = entity.Date, 
            Category = entity.Category,
            Store = entity.Store!,
        };
}
