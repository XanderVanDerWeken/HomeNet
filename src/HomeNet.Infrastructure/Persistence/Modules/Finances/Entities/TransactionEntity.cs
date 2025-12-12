using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;

public sealed class TransactionEntity
{
    public int Id { get; set;}

    public required float Amount { get; set; }

    public required DateTimeOffset Date { get; set; }

    public required Category Category { get; set; }

    public TransactionType Type { get; set; }

    public string? Store { get; set; }

    public string? IncomeSource { get; set; }
}
