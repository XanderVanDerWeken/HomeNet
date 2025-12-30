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
    public void Should_GetMonthlyTimelineAsync()
    {
        // Arrange

        // Act

        // Assert
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
        await _monthlyTimelineRepository.SaveMonthlyTimelineAsync(timeline);

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
