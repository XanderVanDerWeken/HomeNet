using HomeNet.Core.Modules.Finances.Enums;

namespace HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;

public sealed class TransactionEntity
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public decimal Amount { get; set; }

    public TransactionType Type { get; set; }

    public TransactionSource Source { get; set; }

    public string? Description { get; set; }

    public DateOnly Date { get; set; }
}
