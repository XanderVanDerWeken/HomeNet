using System.Data.SQLite;
using Dapper;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Finances;
using SqlKata.Compilers;

namespace HomeNet.Infrastructure.Test.Persistence.Modules.Finances;

public class MonthlyTimelineRepositoryTest
{
    private MonthlyTimelineRepository _monthlyTimelineRepository;

    private SqliteQueryFactory _db;

    [SetUp]
    public void Setup()
    {
        var connectionString = "Data Source=:memory:;Cache=Shared";

        var connection = new SQLiteConnection(connectionString);
        connection.Open();

        var compiler = new SqliteCompiler();

        _db = new SqliteQueryFactory(connection, compiler);

        _monthlyTimelineRepository = new MonthlyTimelineRepository(_db);
    }

    [TearDown]
    public void TearDown()
    {
        _db.Dispose();
        _monthlyTimelineRepository.Dispose();
    }

    [Test]
    public async Task Should_GetMonthlyTimelineAsync()
    {
        // Arrange
        ExecuteSchema();

        var year = 2024;
        int month1 = 6;
        int month2 = 7;

        var timeline1 = new MonthlyTimeline
        {
            Year = year,
            Month = month1,
            IncomeAmount = new Money(2000),
            ExpenseAmount = new Money(1500),
            NetTotal = new Money(500),
        };

        await _monthlyTimelineRepository.SaveMonthlyTimelineAsync(timeline1);

        // Act
        var retrievedTimeline = await _monthlyTimelineRepository.GetMonthlyTimelineAsync(year, month1);
        var missingTimeline = await _monthlyTimelineRepository.GetMonthlyTimelineAsync(year, month2);

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
        ExecuteSchema();

        var timeline = new MonthlyTimeline
        {
            Year = 2024,
            Month = 6,
            IncomeAmount = new Money(2000),
            ExpenseAmount = new Money(1500),
            NetTotal = new Money(500),
        };

        // Act
        Assert.DoesNotThrowAsync(async () =>
        {
            await _monthlyTimelineRepository.SaveMonthlyTimelineAsync(timeline);

            timeline.ExpenseAmount = new Money(1600);
            timeline.NetTotal = new Money(400);

            await _monthlyTimelineRepository.SaveMonthlyTimelineAsync(timeline);
        });
        
        // Assert
    }

    private void ExecuteSchema()
    {
        var schemaPath = "cache.initdb.sql";
        if (!File.Exists(schemaPath))
        {
            throw new FileNotFoundException("SQLite schema file not found", schemaPath);
        }

        var sql = File.ReadAllText(schemaPath);

        _db.Connection.Execute(sql);
    }
}
