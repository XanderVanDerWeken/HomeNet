namespace HomeNet.Core.Modules.Finances.Models;

public sealed record FixedCostWithVersion
{
    public int FixedCostId { get; init; }

    public int FixedCostVersionId { get; init; }

    public int CategoryId { get; init; }

    public required string Name { get; init; }

    public int DayOfMonth { get; init; }

    public Money Amount { get; init; }
}
