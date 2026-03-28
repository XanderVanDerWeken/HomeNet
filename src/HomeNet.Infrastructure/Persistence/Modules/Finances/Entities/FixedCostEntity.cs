namespace HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;

public sealed class FixedCostEntity
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public required string Name { get; set; }

    public int DayOfMonth { get; set; }
}
