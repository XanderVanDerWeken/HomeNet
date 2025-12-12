using HomeNet.Core.Common.Cqrs;

namespace HomeNet.Core.Modules.Cards.Commands;

public sealed record UpdateCardExpiryCommand : ICommand
{
    public required int CardId { get; init; }

    public required DateTimeOffset NewExpiryDate { get; init; }
}
