using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Queries;

public class MonthlyTimelineQueryHandler : IQueryHandler<MonthlyTimelineQuery, MonthlyTimeline>
{
    private readonly ITimelineBuilder _timelineBuilder;

    public MonthlyTimelineQueryHandler(ITimelineBuilder timelineBuilder)
    {
        _timelineBuilder = timelineBuilder;
    }

    public async Task<Result<MonthlyTimeline>> HandleAsync(
        MonthlyTimelineQuery query, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = query.Validate();

        if (!validationResult.IsValid)
        {
            return validationResult.ToFailure<MonthlyTimeline>();
        }

        var timeline = await _timelineBuilder.GetOrCreateMonthlyTimelineAsync(
            query.Year, query.Month, cancellationToken);
        
        return Result<MonthlyTimeline>.Success(timeline);
    }
}
