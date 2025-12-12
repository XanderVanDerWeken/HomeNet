using HomeNet.Core.Common.Cqrs;

namespace HomeNet.Core.Modules.Cards.Commands;

public sealed record RemoveCardCommand : ICommand
{
    public int CardId { get; init; }
}
