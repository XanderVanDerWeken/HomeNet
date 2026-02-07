using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Cache.Modules.Finances;
using Microsoft.Extensions.Caching.Memory;

namespace HomeNet.Infrastructure.Test.Cache.Modules.Finances;

public class MonthlyTimelineCacheTest
{
    private MonthlyTimelineCache _monthlyTimelineCache;

    [SetUp]
    public void Setup()
    {
        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        _monthlyTimelineCache = new MonthlyTimelineCache(
            memoryCache);
    }

    [TearDown]
    public void Teardown()
    {
        _monthlyTimelineCache.Dispose();
    }

    [Test]
    public async Task Should_GetMonthlyTimelineAsync()
    {
        // Arrange
        var year = 2024;
        var month1 = 6;
        var month2 = 7;

        var timeline1 = new MonthlyTimeline
        {
            Year = year,
            Month = month1,
            IncomeAmount = new Money(2000),
            ExpenseAmount = new Money(1500),
            NetTotal = new Money(500),
        };

        await _monthlyTimelineCache.SaveMonthlyTimelineAsync(timeline1);

        // Act
        var retrievedTimeline = await _monthlyTimelineCache.GetMonthlyTimelineAsync(year, month1);
        var missingTimeline = await _monthlyTimelineCache.GetMonthlyTimelineAsync(year, month2);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(retrievedTimeline, Is.Not.Null);
            Assert.That(retrievedTimeline!.Year, Is.EqualTo(timeline1.Year));
            Assert.That(retrievedTimeline.Month, Is.EqualTo(timeline1.Month));
            Assert.That(retrievedTimeline.IncomeAmount, Is.EqualTo(timeline1.IncomeAmount));
            Assert.That(retrievedTimeline.ExpenseAmount, Is.EqualTo(timeline1.ExpenseAmount));
            Assert.That(retrievedTimeline.NetTotal, Is.EqualTo(timeline1.NetTotal));

            Assert.That(missingTimeline, Is.Null);
        });
    }

    [Test]
    public async Task Should_SaveMonthlyTimelineAsync()
    {
        // Arrange
        var timeline = new MonthlyTimeline
        {
            Year = 2024,
            Month = 6,
            IncomeAmount = new Money(2000),
            ExpenseAmount = new Money(1500),
            NetTotal = new Money(500),
        };

        // Act & Assert
        Assert.DoesNotThrowAsync(async () =>
        {
            await _monthlyTimelineCache.SaveMonthlyTimelineAsync(timeline);

            timeline.ExpenseAmount = new Money(1600);
            timeline.NetTotal = new Money(400);

            await _monthlyTimelineCache.SaveMonthlyTimelineAsync(timeline);
        });
    }

    private static string GetCacheKey(int year, int month) 
        => $"monthly_timeline_{year}_{month}";
}
