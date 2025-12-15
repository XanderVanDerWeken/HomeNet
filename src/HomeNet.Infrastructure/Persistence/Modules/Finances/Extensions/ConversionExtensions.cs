using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;

namespace HomeNet.Infrastructure.Persistence.Modules.Finances.Extensions;

public static class ConversionExtensions
{
    public static CategoryEntity ToEntity(this Category category)
        => new CategoryEntity
        {
            Id = category.Id,
            Name = category.Name,
        };

    public static MonthlyTimelineEntity ToEntity(this MonthlyTimeline timeline)
        => new MonthlyTimelineEntity
        {
            Year = timeline.Year,
            Month = timeline.Month,
            IncomeAmount = timeline.IncomeAmount.Amount,
            ExpenseAmount = timeline.ExpenseAmount.Amount,
            NetTotal = timeline.NetTotal.Amount,
        };

    public static TransactionEntity ToEntity(this Income income)
        => new TransactionEntity
        {
            Id = income.Id,
            Amount = income.Amount.Amount,
            Date = income.Date,
            Category = income.Category,
            Type = TransactionType.Income,
            IncomeSource = income.Source,
        };
    
    public static TransactionEntity ToEntity(this Expense expense)
        => new TransactionEntity
        {
            Id = expense.Id,
            Amount = expense.Amount.Amount,
            Date = expense.Date,
            Category = expense.Category,
            Type = TransactionType.Expense,
            Store = expense.Store,
        };
    
    public static Category ToCategory(this CategoryEntity entity)
        => new Category
        {
            Id = entity.Id,
            Name = entity.Name,
        };
    
    public static MonthlyTimeline ToMonthlyTimeline(this MonthlyTimelineEntity entity)
        => new MonthlyTimeline
        {
            Year = entity.Year,
            Month = entity.Month,
            IncomeAmount = new Money(entity.IncomeAmount),
            ExpenseAmount = new Money(entity.ExpenseAmount),
            NetTotal = new Money(entity.NetTotal),
        };

    public static Income ToIncome(this TransactionEntity entity)
        => new Income
        {
            Id = entity.Id,
            Amount = new Money(entity.Amount),
            Date = entity.Date,
            Category = entity.Category,
            Source = entity.IncomeSource!,
        };
    
    public static Expense ToExpense(this TransactionEntity entity)
        => new Expense
        {
            Id = entity.Id,
            Amount = new Money(entity.Amount),
            Date = entity.Date, 
            Category = entity.Category,
            Store = entity.Store!,
        };
}
