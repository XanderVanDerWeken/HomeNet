namespace HomeNet.Core.Modules.Finances.Models;

public sealed class FixedCostWithVersion
{
    public int FixedCostId { get; set; }

    public int FixedCostVersionId { get; set; }

    public int CategoryId { get; set; }

    public required string Name { get; set; }

    public int DayOfMonth { get; set; }

    public Money Amount { get; set; }
}
