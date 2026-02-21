using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Errors;
using HomeNet.Core.Modules.Cards.Abstractions;

namespace HomeNet.Core.Modules.Cards.Commands;

public static class UpdateCardExpiry
{
    public sealed record Command : ICommand
    {
        public required int CardId { get; init; }

        public required DateOnly NewExpiryDate { get; init; }
    }

    public sealed class CommandHandler : ICommandHandler<Command>
    {
        private readonly ICardRepository _cardRepository;

        public CommandHandler(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public async Task<Result> HandleAsync(
            Command command, 
            CancellationToken cancellationToken = default)
        {
            var cardToUpdate = await _cardRepository
                .GetCardByIdAsync(command.CardId, cancellationToken);

            if (cardToUpdate is null)
            {
                return new NotFoundError("Card", command.CardId).ToFailure();
            }

            cardToUpdate.ExpirationDate = command.NewExpiryDate;
            return await _cardRepository
                .UpdateCardAsync(cardToUpdate, cancellationToken);
        }
    }
}
