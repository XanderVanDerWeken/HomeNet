using HomeNet.Core.Modules.Finances.Enums;

namespace HomeNet.Core.Modules.Finances.Models;

public sealed class Transaction
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public Money Amount { get; set; }

    public TransactionType Type { get; set; }

    public TransactionSource Source { get; set; }

    public string? Description { get; set; }

    public required DateOnly Date { get; set; }
}
