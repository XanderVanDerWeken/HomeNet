namespace HomeNet.Core.Modules.Finances.Models;

public sealed class MonthlyTimeline
{
    public required int Year { get; set; }

    public required int Month { get; set; }

    public required float IncomeAmount { get; set; }

    public required float ExpenseAmount { get; set; }

    public required float NetTotal { get; set; }
}
