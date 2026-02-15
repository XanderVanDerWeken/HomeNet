namespace HomeNet.Core.Modules.Finances.Models;

public sealed class FixedCostInterval
{
    public int Id { get; set; }

    public required Money Amount { get; set; }

    public required DateOnly BeginDate { get; set; }

    public DateOnly? EndDate { get; set; }
}
