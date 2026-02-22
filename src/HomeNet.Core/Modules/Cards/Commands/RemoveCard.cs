using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Cards.Abstractions;

namespace HomeNet.Core.Modules.Cards.Commands;

public static class RemoveCard
{
    public sealed record Command : ICommand
    {
        public int CardId { get; init; }
    }

    public sealed class CommandHandler : ICommandHandler<Command>
    {
        private readonly ICardRepository _cardRepository;

        public CommandHandler(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public Task<Result> HandleAsync(
            Command command,
            CancellationToken cancellationToken = default)
        {
            return _cardRepository.RemoveCardAsync(
                command.CardId, cancellationToken);
        }
    }
}
