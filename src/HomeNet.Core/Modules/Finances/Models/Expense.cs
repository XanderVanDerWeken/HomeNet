namespace HomeNet.Core.Modules.Finances.Models;

public sealed class Expense : Transaction
{
    public required string Store { get; set; }
}
