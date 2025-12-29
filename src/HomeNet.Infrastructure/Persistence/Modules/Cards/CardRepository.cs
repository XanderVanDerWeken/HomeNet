using HomeNet.Core.Common;
using HomeNet.Core.Modules.Cards.Abstractions;
using HomeNet.Core.Modules.Cards.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Cards.Entities;
using HomeNet.Infrastructure.Persistence.Modules.Cards.Extensions;
using SqlKata;

namespace HomeNet.Infrastructure.Persistence.Modules.Cards;

public sealed class CardRepository : SqlKataRepository, ICardRepository
{
    private static readonly string TableName = "cards.cards";

    public CardRepository(PostgresQueryFactory db)
        : base(db)
    {
    }

    public async Task<Result> AddCardAsync(
        Card card, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new Query(TableName).AsInsert(new
            {
                name = card.Name,
                expiration_date = card.ExpirationDate,
                person_id = card.PersonId,
            });

            var newCardId = await InsertAndReturnIdAsync(query);
            card.Id = newCardId;

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while adding the card: {ex.Message}");
        }
    }

    public async Task<Card?> GetCardByIdAsync(
        int cardId, 
        CancellationToken cancellationToken = default)
    {
        var query = new Query(TableName)
            .Where("id", cardId);
        
        var entity = await FirstOrDefaultAsync<CardEntity>(
            query, 
            cancellationToken);
        
        return entity?.ToCard();
    }

    public async Task<IReadOnlyList<Card>> GetAllCardsAsync(
        CancellationToken cancellationToken = default)
    {
        var query = new Query(TableName);

        var entities = await GetMultipleAsync<CardEntity>(
            query, 
            cancellationToken);
        
        return entities
            .Select(e => e.ToCard())
            .ToList();
    }

    public async Task<IReadOnlyList<Card>> GetAllCardsWithExpiryBeforeAsync(
        DateOnly expiryDate, 
        CancellationToken cancellationToken = default)
    {
        var query = new Query(TableName)
            .Where("expiration_date", "<", expiryDate);

        var entities = await GetMultipleAsync<CardEntity>(
            query, 
            cancellationToken);
        
        return entities
            .Select(e => e.ToCard())
            .ToList();
    }

    public async Task<Result> UpdateCardAsync(
        Card card, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new Query(TableName)
                .Where("id", card.Id)
                .AsUpdate(new
                {
                    name = card.Name,
                    expiration_date = card.ExpirationDate,
                });

            var affectedRows = await ExecuteAsync(query, cancellationToken);

            return affectedRows > 0
                ? Result.Success()
                : Result.Failure("Failed to update card in database.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while updating the card: {ex.Message}");
        }
    }

    public async Task<Result> RemoveCardAsync(
        int cardId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new Query(TableName)
                .Where("id", cardId)
                .AsDelete();

            var affectedRows = await ExecuteAsync(query, cancellationToken);

            return affectedRows > 0
                ? Result.Success()
                : Result.Failure("Failed to delete card from database.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while removing the card: {ex.Message}");
        }
    }
}
