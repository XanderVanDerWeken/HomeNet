namespace HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;

public sealed class MonthlyTimelineEntity
{
    public required int Year { get; set; }

    public required int Month { get; set; }

    public required decimal IncomeAmount { get; set; }

    public required decimal ExpenseAmount { get; set; }

    public required decimal NetTotal { get; set; }
}
