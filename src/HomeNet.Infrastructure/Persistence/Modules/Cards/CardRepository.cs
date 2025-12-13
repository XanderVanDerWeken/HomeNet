using HomeNet.Core.Common;
using HomeNet.Core.Modules.Cards.Abstractions;
using HomeNet.Core.Modules.Cards.Models;
using SqlKata;
using SqlKata.Execution;

namespace HomeNet.Infrastructure.Persistence.Modules.Cards;

public sealed class CardRepository : SqlKataRepository, ICardRepository
{
    private static readonly string TableName = "cards";

    public CardRepository(QueryFactory db)
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
                expirationDate = card.ExpirationDate,
            });

            var rows = await ExecuteAsync(query, cancellationToken: cancellationToken);

            return rows > 0
                ? Result.Success()
                : Result.Failure("Failed to insert card into database.");
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
        
        var row = await FirstOrDefaultAsync<Card>(query, cancellationToken: cancellationToken);
        return row;
    }

    public async Task<IReadOnlyList<Card>> GetAllCardsAsync(
        CancellationToken cancellationToken = default)
    {
        var query = new Query(TableName);

        return await GetListAsync<Card>(query, cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<Card>> GetAllCardsWithExpiryBeforeAsync(
        DateTimeOffset expiryDate, 
        CancellationToken cancellationToken = default)
    {
        var query = new Query(TableName)
            .Where("expiration_date", "<", expiryDate);

        return await GetListAsync<Card>(query, cancellationToken: cancellationToken);
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
                    expirationDate = card.ExpirationDate,
                });

            var rows = await ExecuteAsync(query, cancellationToken: cancellationToken);

            return rows > 0
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

            var rows = await ExecuteAsync(query, cancellationToken: cancellationToken);

            return rows > 0
                ? Result.Success()
                : Result.Failure("Failed to delete card from database.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while removing the card: {ex.Message}");
        }
    }
}
