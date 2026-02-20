namespace HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;

public sealed class FixedCostIntervalEntity
{
    public int Id { get; set; }

    public int FixedCostId { get; set; }

    public required decimal Amount { get; set; }

    public int StartYear { get; set; }

    public int StartMonth { get; set; }

    public int EndYear { get; set; }

    public int EndMonth { get; set; }
}
