using System;
using HomeNet.Core.Common;
using HomeNet.Core.Modules.Cards.Abstractions;
using HomeNet.Core.Modules.Cards.Models;

namespace HomeNet.Infrastructure.Persistence.Modules.Cards;

public sealed class CardRepository : ICardRepository
{
    private readonly AppDbContext _dbContext;

    public CardRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Card>> AddCardAsync(Card card, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(card, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(card);
    }

    public Task<IReadOnlyList<Card>> GetAllCardsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Card>> GetAllCardsWithExpiryBeforeAsync(DateOnly expiryDate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Card?> GetCardByIdAsync(int cardId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Unit>> RemoveCardAsync(int cardId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Card>> UpdateCardAsync(Card card, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
