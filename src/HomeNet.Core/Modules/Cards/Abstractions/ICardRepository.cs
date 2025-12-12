using HomeNet.Core.Common;
using HomeNet.Core.Modules.Cards.Models;

namespace HomeNet.Core.Modules.Cards.Abstractions;

public interface ICardRepository
{
    Task<Result> AddCardAsync(
        Card card, 
        CancellationToken cancellationToken = default);
    
    Task<Result> UpdateCardAsync(
        Card card, 
        CancellationToken cancellationToken = default);

    Task<Result> RemoveCardAsync(
        int cardId, 
        CancellationToken cancellationToken = default);

    Task<Card?> GetCardByIdAsync(
        int cardId, 
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Card>> GetAllCardsAsync(
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<Card>> GetAllCardsWithExpiryBeforeAsync(
        DateTimeOffset expiryDate, 
        CancellationToken cancellationToken = default);
}
