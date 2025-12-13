using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Commands;

public sealed class RecalculateMonthlyTimelineCommandHandler : ICommandHandler<RecalculateMonthlyTimelineCommand>
{
    public Task<Result> HandleAsync(
        RecalculateMonthlyTimelineCommand command, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
