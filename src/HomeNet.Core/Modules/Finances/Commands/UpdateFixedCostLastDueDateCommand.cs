using HomeNet.Core.Common.Cqrs;

namespace HomeNet.Core.Modules.Finances.Commands;

public sealed record UpdateFixedCostLastDueDateCommand : ICommand
{
    public required int FixedCostId { get; init; }

    public required DateOnly LastDueDate { get; init; }
}
