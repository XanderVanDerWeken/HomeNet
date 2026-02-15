using HomeNet.Core.Common.Cqrs;

namespace HomeNet.Core.Modules.Finances.Commands;

public sealed record RemoveFixedCostCommand : ICommand
{
    public required int FixedCostId { get; init; }
}
