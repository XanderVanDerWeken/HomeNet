namespace HomeNet.Core.Modules.Finances.Models;

public sealed record FixedCostTransaction
{
    public int Id { get; init; }

    public int FixedCostId { get; init; }

    public int FixedCostVersionId { get; init; }

    public int TransactionId { get; init; }

    public DateOnly Period { get; init; }
}
