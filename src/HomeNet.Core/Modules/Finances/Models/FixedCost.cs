namespace HomeNet.Core.Modules.Finances.Models;

public sealed class FixedCost
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required Money Amount { get; set; }

    public required DateOnly FirstDueDate { get; set; }

    public DateOnly? LastDueDate { get; set; } = null;
}
