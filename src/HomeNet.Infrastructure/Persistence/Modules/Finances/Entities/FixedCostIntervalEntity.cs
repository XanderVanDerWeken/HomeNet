namespace HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;

public sealed class FixedCostIntervalEntity
{
    public int Id { get; set; }

    public int FixedCostId { get; set; }

    public required decimal Amount { get; set; }

    public required DateOnly BeginDate { get; set; }

    public DateOnly? EndDate { get; set; } = null;
}
