using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Commands;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Queries;

public class MonthlyTimelineQueryHandler : IQueryHandler<MonthlyTimelineQuery, MonthlyTimeline>
{
    private readonly IMonthlyTimelineRepository _monthlyTimelineRepository;


    public Task<Result<MonthlyTimeline>> HandleAsync(
        MonthlyTimelineQuery query, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
