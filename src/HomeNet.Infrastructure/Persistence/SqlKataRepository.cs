using SqlKata;
using SqlKata.Execution;

namespace HomeNet.Infrastructure.Persistence;

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
        => _db.ExecuteAsync(query, cancellationToken: cancellationToken);

    protected Task<T> FirstOrDefaultAsync<T>(
        Query query, 
        CancellationToken cancellationToken = default)
        => _db.FirstOrDefaultAsync<T>(query, cancellationToken: cancellationToken);

    protected async Task<IReadOnlyList<T>> GetListAsync<T>(
        Query query, 
        CancellationToken cancellationToken = default)
    {
        var rows = await _db.GetAsync<T>(query, cancellationToken: cancellationToken);
        return rows.ToList();
    }

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
