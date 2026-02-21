using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Cards.Abstractions;
using HomeNet.Core.Modules.Cards.Models;

namespace HomeNet.Core.Modules.Cards.Queries;

public static class AllCardsExpiringBefore
{
    public sealed record Query : IQuery
    {
        public DateOnly ExpiryDate { get; init; }
    }

    public sealed class QueryHandler : IQueryHandler<Query, IReadOnlyList<Card>>
    {
        private readonly ICardRepository _cardRepository;

        public QueryHandler(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public async Task<Result<IReadOnlyList<Card>>> HandleAsync(
            Query query, CancellationToken cancellationToken = default)
        {
            var cards = await _cardRepository.GetAllCardsWithExpiryBeforeAsync(
                query.ExpiryDate, 
                cancellationToken);

            return Result<IReadOnlyList<Card>>.Success(cards);
        }
    }
}
