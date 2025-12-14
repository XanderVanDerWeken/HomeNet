namespace HomeNet.Core.Modules.Finances.Models;

public sealed class MonthlyTimeline
{
    public required int Year { get; set; }

    public required int Month { get; set; }

    public required Money IncomeAmount { get; set; }

    public required Money ExpenseAmount { get; set; }

    public required Money NetTotal { get; set; }
}
