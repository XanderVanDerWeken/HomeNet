using HomeNet.Core.Common.Cqrs;

namespace HomeNet.Core.Modules.Cards.Commands;

public sealed record AddCardCommand : ICommand
{
    public required string Name { get; init; }

    public required DateTime ExpirationDate { get; init; }
}
