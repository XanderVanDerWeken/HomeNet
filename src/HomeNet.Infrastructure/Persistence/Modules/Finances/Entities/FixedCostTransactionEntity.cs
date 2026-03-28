namespace HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;

public sealed class FixedCostTransactionEntity
{
    public int Id { get; set; }

    public int FixedCostId { get; set; }

    public int FixedCostVersionId { get; set; }

    public int TransactionId { get; set; }

    public DateOnly Period { get; set; }
}
