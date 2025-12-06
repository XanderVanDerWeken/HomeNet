using Dapper;
using HomeNet.Core.Common;
using HomeNet.Core.Modules.Cards.Abstractions;
using HomeNet.Core.Modules.Cards.Models;
using Npgsql;
using SqlKata;
using SqlKata.Execution;

namespace HomeNet.Infrastructure.Persistence.Modules.Cards;

public sealed class CardRepository : ICardRepository, IDisposable
{
    private readonly QueryFactory _db;

    private bool _disposed = false;

    public CardRepository(QueryFactory db)
    {
        _db = db;
    }

    ~CardRepository()
    {
        Dispose(false);
    }

    public async Task<Result> AddCardAsync(
        Card card, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new Query("cards").AsInsert(new
            {
                name = card.Name,
                expirationDate = card.ExpirationDate,
            });

            var compiled = _db.Compiler.Compile(query);

            using var connection = new NpgsqlConnection(_db.Connection.ConnectionString);
            var rows = await connection.ExecuteAsync(compiled.Sql, compiled.NamedBindings);

            return rows > 0
                ? Result.Success()
                : Result.Failure("Failed to insert card into database.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while adding the card: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<Card>> GetAllCardsAsync(
        CancellationToken cancellationToken = default)
    {
        var query = new Query("cards").OrderBy("id");

        var rows = await _db.GetAsync<Card>(query, cancellationToken: cancellationToken);
        return rows.ToList();
    }

    public async Task<IReadOnlyList<Card>> GetAllCardsWithExpiryBeforeAsync(
        DateTimeOffset expiryDate, 
        CancellationToken cancellationToken = default)
    {
        var query = new Query("cards")
            .Where("expiration_date", "<", expiryDate)
            .OrderBy("id");

        var rows = await _db.GetAsync<Card>(query, cancellationToken: cancellationToken);
        return rows.ToList();
    }

    public async Task<Result> RemoveCardAsync(
        int cardId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new Query("cards")
                .Where("id", cardId)
                .AsDelete();

            var compiled = _db.Compiler.Compile(query);

            using var connection = new NpgsqlConnection(_db.Connection.ConnectionString);
            var rows = await connection.ExecuteAsync(compiled.Sql, compiled.NamedBindings);

            return rows > 0
                ? Result.Success()
                : Result.Failure("Failed to delete card from database.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while removing the card: {ex.Message}");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        
        if (disposing)
        {
            _db.Dispose();
        }

        _disposed = true;
    }
}
