namespace HomeNet.Core.Modules.Finances.Models;

public abstract class Transaction
{
    public int Id { get; set;}

    public required float Amount { get; set; }

    public required DateTimeOffset Date { get; set; }

    public required Category Category { get; set; }
}
