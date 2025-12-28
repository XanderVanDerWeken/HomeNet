using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Abstractions;

public interface ITimelineBuilder
{
    Task<MonthlyTimeline> GetOrCreateMonthlyTimelineAsync(
        int year, 
        int month,
        CancellationToken cancellationToken = default);
    
    Task<MonthlyTimeline> RecalculateMonthlyTimelineAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default);

    Task<MonthlyTimeline> BuildMonthlyTimelineAsync(
        int year, 
        int month,
        CancellationToken cancellationToken = default);
}
