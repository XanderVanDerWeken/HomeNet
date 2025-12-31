using Dapper;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Extensions;
using SqlKata;

namespace HomeNet.Infrastructure.Persistence.Modules.Finances;

public sealed class MonthlyTimelineRepository : SqlKataRepository, IMonthlyTimelineRepository
{
    private static readonly string TableName = "monthly_timelines";

    public MonthlyTimelineRepository(SqliteQueryFactory db) 
        : base(db)
    {
    }

    public async Task<MonthlyTimeline?> GetMonthlyTimelineAsync(
        int year, 
        int month, 
        CancellationToken cancellationToken = default)
    {
        var query = new Query(TableName)
            .Where("year", year)
            .Where("month", month);

        var row = await FirstOrDefaultAsync<MonthlyTimelineEntity>(
            query, 
            cancellationToken);
        return row?.ToMonthlyTimeline();
    }

    public async Task SaveMonthlyTimelineAsync(
        MonthlyTimeline timeline, 
        CancellationToken cancellationToken = default)
    {
        await _db.Connection.ExecuteAsync(UpsertMonthlyTimelineSql, timeline.ToEntity());
    }

    private const string UpsertMonthlyTimelineSql = """
INSERT INTO monthly_timelines (year, month, income_amount, expense_amount, net_total)
VALUES (@Year, @Month, @IncomeAmount, @ExpenseAmount, @NetTotal)
ON CONFLICT (year, month)
DO UPDATE SET
    income_amount = excluded.income_amount,
    expense_amount = excluded.expense_amount,
    net_total = excluded.net_total;    
""";
}
