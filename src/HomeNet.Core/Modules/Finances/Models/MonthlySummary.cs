namespace HomeNet.Core.Modules.Finances.Models;

public sealed class MonthlySummary
{
    public int Year { get; set; }

    public int Month { get; set; }

    public Money TotalIncome { get; set; }

    public Money TotalExpenses { get; set; }

    public Money Balance => TotalIncome - TotalExpenses;

    public IReadOnlyList<CategorySummary> ByCategory { get; set; } = [];
}
