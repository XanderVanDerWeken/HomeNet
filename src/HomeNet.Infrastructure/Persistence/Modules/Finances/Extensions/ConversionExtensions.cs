using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;

namespace HomeNet.Infrastructure.Persistence.Modules.Finances.Extensions;

public static class ConversionExtensions
{
    public static MonthlyTimelineEntity ToEntity(this MonthlyTimeline timeline)
        => new MonthlyTimelineEntity
        {
            Year = timeline.Year,
            Month = timeline.Month,
            IncomeAmount = timeline.IncomeAmount.Amount,
            ExpenseAmount = timeline.ExpenseAmount.Amount,
            NetTotal = timeline.NetTotal.Amount,
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

    public static Income ToIncome(this TransactionEntity entity, Category category)
        => new Income
        {
            Id = entity.Id,
            Amount = new Money(entity.Amount),
            Date = entity.Date,
            Category = category,
            Source = entity.IncomeSource!,
        };
    
    public static Expense ToExpense(this TransactionEntity entity, Category category)
        => new Expense
        {
            Id = entity.Id,
            Amount = new Money(entity.Amount),
            Date = entity.Date, 
            Category = category,
            Store = entity.Store!,
        };
}
