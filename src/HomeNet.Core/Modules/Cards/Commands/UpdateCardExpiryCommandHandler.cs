using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Cards.Abstractions;

namespace HomeNet.Core.Modules.Cards.Commands;

public class UpdateCardExpiryCommandHandler : ICommandHandler<UpdateCardExpiryCommand>
{
    private readonly ICardRepository _cardRepository;

    public UpdateCardExpiryCommandHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task<Result> HandleAsync(UpdateCardExpiryCommand command, CancellationToken cancellationToken = default)
    {
        var cardToUpdate = await _cardRepository.GetCardByIdAsync(command.CardId, cancellationToken);

        if (cardToUpdate is null)
        {
            return Result.Failure($"Card with ID {command.CardId} not found.");
        }

        cardToUpdate.ExpirationDate = command.NewExpiryDate;
        return await _cardRepository.UpdateCardAsync(cardToUpdate, cancellationToken);
    }
}
