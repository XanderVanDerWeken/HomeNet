using HomeNet.Core.Common;
using HomeNet.Core.Common.Errors;
using HomeNet.Core.Modules.Cards.Abstractions;
using HomeNet.Core.Modules.Cards.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeNet.Infrastructure.Persistence.Modules.Cards;

public sealed class CardRepository : ICardRepository
{
    private readonly CardDbContext _dbContext;

    public CardRepository(CardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Card>> AddCardAsync(Card card, CancellationToken cancellationToken = default)
    {
        await _dbContext.Cards.AddAsync(card, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(card);
    }

    public async Task<IReadOnlyList<Card>> GetAllCardsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Cards
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Card>> GetAllCardsWithExpiryBeforeAsync(DateOnly expiryDate, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Cards
            .AsNoTracking()
            .Where(c => c.ExpirationDate < expiryDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Card?> GetCardByIdAsync(int cardId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Cards
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == cardId, cancellationToken);
    }

    public async Task<Result<Unit>> RemoveCardAsync(int cardId, CancellationToken cancellationToken = default)
    {
        var card = await GetCardByIdAsync(cardId, cancellationToken);

        if (card is null)
        {
            return new NotFoundError(nameof(Card), cardId)
                .ToFailure<Unit>();
        }

        _dbContext.Cards.Remove(card);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }

    public async Task<Result<Card>> UpdateCardAsync(Card card, CancellationToken cancellationToken = default)
    {
        var affectedRows = await _dbContext.Cards
            .Where(c => c.Id == card.Id)
            .ExecuteUpdateAsync(update => update
                .SetProperty(c => c.Name, card.Name)
                .SetProperty(c => c.ExpirationDate, card.ExpirationDate)
                .SetProperty(c => c.PersonId, card.PersonId),
                cancellationToken);
        
        return affectedRows > 0
            ? Result.Success(card)
            : new NotFoundError(nameof(Card), card.Id).ToFailure<Card>();
    }
}
