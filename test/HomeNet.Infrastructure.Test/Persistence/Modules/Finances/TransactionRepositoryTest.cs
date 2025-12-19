using HomeNet.Core.Common;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Finances;
using HomeNet.Infrastructure.Test.Containers;
using Npgsql;
using NUnit.Framework;
using SqlKata.Compilers;

namespace HomeNet.Infrastructure.Test.Persistence.Modules.Finances;

public class TransactionRepositoryTest
{
    private TransactionRepository _transactionRepository;

    private HomenetPgContainer _dbContainer;

    [SetUp]
    public async Task Setup()
    {
        _dbContainer = new HomenetPgContainer();
        await _dbContainer.StartAsync();

        var connectionString = _dbContainer.GetConnectionString();

        var connection = new NpgsqlConnection(connectionString);
        var compiler = new PostgresCompiler();

        var db = new PostgresQueryFactory(connection, compiler);

        _transactionRepository = new TransactionRepository(db);
    }

    [TearDown]
    public async Task Teardown()
    {
        _transactionRepository.Dispose();

        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_AddExpenseAsync()
    {
        // Arrange
        var category = new Category
        {
            Name = "Other",
        };

        var expense = new Expense
        {
            Amount = new Money(50),
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Category = category,
            Store = "Supermarket",
        };

        // Act
        Result? result = null;
        Assert.DoesNotThrowAsync(async () =>
        {
            result = await _transactionRepository.AddExpenseAsync(expense);
        });
        

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsSuccess, Is.True);
            Assert.That(result.Error, Is.Null);
        });
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_AddIncomeAsync()
    {
        // Arrange

        // Act

        // Assert
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_GetAllExpensesAsync()
    {
        // Arrange

        // Act

        // Assert
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_GetAllIncomesAsync()
    {
        // Arrange

        // Act

        // Assert
    }
}
