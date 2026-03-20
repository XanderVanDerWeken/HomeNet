namespace HomeNet.Core.Modules.Finances.Models;

public sealed class FixedCostVersion
{
    public int Id { get; set; }

    public int FixedCostId { get; set; }

    public Money Amount { get; set; }

    public DateOnly ValidFrom { get; set; }

    public DateOnly? ValidTo { get; set; }
}
