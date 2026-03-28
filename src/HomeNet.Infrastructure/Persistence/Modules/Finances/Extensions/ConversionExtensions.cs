using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;

namespace HomeNet.Infrastructure.Persistence.Modules.Finances.Extensions;

public static class ConversionExtensions
{
    public static Category ToCategory(this CategoryEntity entity)
        => new Category
        {
            Id = entity.Id,
            Name = entity.Name,
        };

    public static Transaction ToTransaction(this TransactionEntity entity)
        => new Transaction
        {
            Id = entity.Id,
            Date = entity.Date,
            Amount = new Money(entity.Amount),
            Type = entity.Type,
            CategoryId = entity.CategoryId,
        };
    
    public static FixedCost ToFixedCost(this FixedCostEntity entity)
        => new FixedCost
        {
            Id = entity.Id,
            Name = entity.Name,
            CategoryId = entity.CategoryId,
            DayOfMonth = entity.DayOfMonth,
        };
    
    public static FixedCostVersion ToFixedCostVersion(this FixedCostVersionEntity entity)
        => new FixedCostVersion
        {
            Id = entity.Id,
            FixedCostId = entity.FixedCostId,
            Amount = new Money(entity.Amount),
            ValidFrom = entity.ValidFrom,
            ValidTo = entity.ValidTo,
        };
    public static FixedCostTransaction ToFixedCostTransaction(this FixedCostTransactionEntity entity)
        => new FixedCostTransaction
        {
            Id = entity.Id,
            FixedCostId = entity.FixedCostId,
            FixedCostVersionId = entity.FixedCostVersionId,
            TransactionId = entity.TransactionId,
            Period = entity.Period,
        };
}
