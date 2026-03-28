namespace HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;

public sealed class FixedCostVersionEntity
{
    public int Id { get; set; }

    public int FixedCostId { get; set; }

    public decimal Amount { get; set; }

    public DateOnly ValidFrom { get; set; }

    public DateOnly? ValidTo { get; set; }
}
