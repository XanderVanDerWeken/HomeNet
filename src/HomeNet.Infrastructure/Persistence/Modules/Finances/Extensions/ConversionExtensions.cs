using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;

namespace HomeNet.Infrastructure.Persistence.Modules.Finances.Extensions;

public static class ConversionExtensions
{
    public static Category ToCategory(this CategoryEntity entity)
        => new Category
        {
            Id = entity.Id,
            Name = entity.Name
        };

    public static Transaction ToTransaction(this TransactionEntity entity)
        => new Transaction
        {
            Id = entity.Id,
            Date = entity.Date,
            Amount = new Money(entity.Amount),
            Type = entity.Type,
            CategoryId = entity.CategoryId
        };
}
