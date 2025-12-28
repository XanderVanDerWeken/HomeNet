namespace HomeNet.Core.Modules.Finances.Models;

public abstract class Transaction
{
    public int Id { get; set;}

    public required Money Amount { get; set; }

    public required DateOnly Date { get; set; }

    public required Category Category { get; set; }
}
