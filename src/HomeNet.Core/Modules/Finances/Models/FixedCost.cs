namespace HomeNet.Core.Modules.Finances.Models;

public sealed class FixedCost
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public required string Name { get; set; }

    public int DayOfMonth { get; set; }
}
