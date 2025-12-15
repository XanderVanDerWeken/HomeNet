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
        var existingTimeline = await GetMonthlyTimelineAsync(
            timeline.Year, 
            timeline.Month, 
            cancellationToken);

        Query query;
        if (existingTimeline == null)
        {
            query = new Query(TableName)
                .AsInsert(timeline.ToEntity());
        }
        else
        {
            query = new Query(TableName)
                .Where("year", timeline.Year)
                .Where("month", timeline.Month)
                .AsUpdate(timeline.ToEntity());
        }

        await ExecuteAsync(
            query, 
            cancellationToken);
    }
}
