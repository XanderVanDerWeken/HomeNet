using HomeNet.Core.Modules.Finances.Enums;

namespace HomeNet.Core.Modules.Finances.Models;

public sealed class CategorySummary
{
    public int CategoryId { get; set; }

    public required string CategoryName { get; set; }

    public TransactionType TransactionType { get; set; }

    public int TransactionCount { get; set; }

    public Money TotalAmount { get; set; }
}
