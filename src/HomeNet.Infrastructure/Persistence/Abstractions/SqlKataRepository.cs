using System.Data;
using Dapper;
using SqlKata;
using SqlKata.Execution;

namespace HomeNet.Infrastructure.Persistence.Abstractions;

public abstract class SqlKataRepository : IDisposable
{
    protected readonly QueryFactory _db;
    private bool _disposed = false;

    protected SqlKataRepository(QueryFactory db)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        _db = db;
    }

    ~SqlKataRepository()
    {
        Dispose(false);
    }

    protected Task<int> ExecuteAsync(
        Query query, 
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
        => _db.ExecuteAsync(
            query,
            transaction: transaction,
            cancellationToken: cancellationToken);
    
    protected Task<int> InsertAndReturnIdAsync(
        Query query,
        IDbTransaction? transaction = null)
    {
        var compiled = _db.Compiler.Compile(query);

        var id = _db.Connection.QuerySingleAsync<int>(
            compiled.Sql + " RETURNING id",
            compiled.NamedBindings,
            transaction: transaction);
        
        return id;
    }

    protected Task<T> FirstOrDefaultAsync<T>(
        Query query,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
        => _db.FirstOrDefaultAsync<T>(
            query,
            transaction: transaction,
            cancellationToken: cancellationToken);

    protected Task<IEnumerable<T>> GetMultipleAsync<T>(
        Query query,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
        => _db.GetAsync<T>(
            query,
            transaction: transaction,
            cancellationToken: cancellationToken);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
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
