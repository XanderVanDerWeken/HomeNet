namespace HomeNet.Core.Modules.Finances.Models;

public sealed class FixedCostTransaction
{
    public int Id { get; set; }

    public int FixedCostId { get; set; }

    public int FixedCostVersionId { get; set; }

    public int TransactionId { get; set; }

    public DateOnly Period { get; set; }
}
