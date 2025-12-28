namespace HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;

public sealed class TransactionEntity
{
    public int Id { get; set;}

    public required decimal Amount { get; set; }

    public required DateOnly Date { get; set; }

    public required int CategoryId { get; set; }

    public TransactionType Type { get; set; }

    public string? Store { get; set; }

    public string? IncomeSource { get; set; }
}
