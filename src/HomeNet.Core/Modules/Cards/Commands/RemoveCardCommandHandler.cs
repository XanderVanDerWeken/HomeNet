using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Cards.Abstractions;

namespace HomeNet.Core.Modules.Cards.Commands;

public sealed class RemoveCardCommandHandler : ICommandHandler<RemoveCardCommand>
{
    private readonly ICardRepository _cardRepository;

    public RemoveCardCommandHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public Task<Result> HandleAsync(
        RemoveCardCommand command, 
        CancellationToken cancellationToken = default)
    {
        return _cardRepository.RemoveCardAsync(command.CardId, cancellationToken);
    }
}
