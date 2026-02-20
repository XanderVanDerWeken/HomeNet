namespace HomeNet.Core.Modules.Finances.Models;

public sealed class FixedCostInterval
{
    public int Id { get; set; }

    public required Money Amount { get; set; }

    public int StartYear { get; set; }

    public int StartMonth { get; set; }

    public int EndYear { get; set; }

    public int EndMonth { get; set; }
}
