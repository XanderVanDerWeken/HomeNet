using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;
using Microsoft.Extensions.Caching.Memory;

namespace HomeNet.Infrastructure.Cache.Modules.Finances;

public sealed class MonthlyTimelineCache : IMonthlyTimelineRepository, IDisposable
{
    private readonly IMemoryCache _memoryCache;

    private bool _disposed;

    public MonthlyTimelineCache(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    ~MonthlyTimelineCache()
    {
        Dispose(false);
    }

    public Task<MonthlyTimeline?> GetMonthlyTimelineAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        var key = GetCacheKey(year, month);
        if (_memoryCache.TryGetValue(key, out MonthlyTimeline? timeline))
        {
            return Task.FromResult(timeline);
        }

        return Task.FromResult<MonthlyTimeline?>(null);
    }

    public Task SaveMonthlyTimelineAsync(MonthlyTimeline timeline, CancellationToken cancellationToken = default)
    {
        var key = GetCacheKey(timeline.Year, timeline.Month);
        _memoryCache.Set(key, timeline);
        
        return Task.CompletedTask;
    }

    private static string GetCacheKey(int year, int month) 
        => $"monthly_timeline_{year}_{month}";

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
            _memoryCache.Dispose();
        }

        _disposed = true;
    }
}
