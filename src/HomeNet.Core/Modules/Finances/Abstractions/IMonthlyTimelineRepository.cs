using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Abstractions;

public interface IMonthlyTimelineRepository
{
    Task<MonthlyTimeline?> GetMonthlyTimelineAsync(
        int year, 
        int month,
        CancellationToken cancellationToken = default);
    
    Task SaveMonthlyTimelineAsync(
        MonthlyTimeline timeline, 
        CancellationToken cancellationToken = default);
}
