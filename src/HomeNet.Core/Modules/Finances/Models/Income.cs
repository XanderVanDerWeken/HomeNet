namespace HomeNet.Core.Modules.Finances.Models;

public sealed class Income : Transaction
{
    public required string Source { get; set; }
}
