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
        _db = db;
    }

    ~SqlKataRepository()
    {
        Dispose(false);
    }

    protected Task<int> ExecuteAsync(
        Query query, 
        CancellationToken cancellationToken = default)
        => _db.ExecuteAsync(
            query, 
            cancellationToken: cancellationToken);
    
    protected Task<int> InsertAndReturnIdAsync(
        Query query)
    {
        var compiled = _db.Compiler.Compile(query);

        var id = _db.Connection.QuerySingleAsync<int>(
            compiled.Sql + " RETURNING id",
            compiled.NamedBindings);
        
        return id;
    }

    protected Task<T> FirstOrDefaultAsync<T>(
        Query query, 
        CancellationToken cancellationToken = default)
        => _db.FirstOrDefaultAsync<T>(
            query, 
            cancellationToken: cancellationToken);

    protected Task<IEnumerable<T>> GetMultipleAsync<T>(
        Query query, 
        CancellationToken cancellationToken = default)
        => _db.GetAsync<T>(
            query, 
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
