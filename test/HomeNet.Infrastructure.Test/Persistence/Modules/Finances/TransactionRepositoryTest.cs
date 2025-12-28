using HomeNet.Core.Common;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Finances;
using HomeNet.Infrastructure.Test.Containers;
using Npgsql;
using SqlKata.Compilers;

namespace HomeNet.Infrastructure.Test.Persistence.Modules.Finances;

public class TransactionRepositoryTest
{
    private TransactionRepository _transactionRepository;
    private CategoryRepository _categoryRepository;

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

        _categoryRepository = new CategoryRepository(db);

        _transactionRepository = new TransactionRepository(db, _categoryRepository);
    }

    [TearDown]
    public async Task Teardown()
    {
        _transactionRepository.Dispose();
        _categoryRepository.Dispose();

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
        var categoryAdded = await _categoryRepository.AddCategoryAsync(category);

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
            Assert.That(categoryAdded.IsSuccess, Is.True);

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
        var category = new Category
        {
            Name = "Other",
        };
        var categoryAdded = await _categoryRepository.AddCategoryAsync(category);

        var income = new Income
        {
            Amount = new Money(50),
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Category = category,
            Source = "Company",
        };

        // Act
        Result? result = null;
        Assert.DoesNotThrowAsync(async () =>
        {
            result = await _transactionRepository.AddIncomeAsync(income);
        });
        

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(categoryAdded.IsSuccess, Is.True);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsSuccess, Is.True);
            Assert.That(result.Error, Is.Null);
        });
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_GetAllExpensesAsync()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var category = new Category
        {
            Name = "Other",
        };
        var categoryAdded = await _categoryRepository.AddCategoryAsync(category);

        var expense1 = new Expense
        {
            Amount = new Money(50),
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Category = category,
            Store = "Supermarket",
        };
        var expense1Added = await _transactionRepository.AddExpenseAsync(expense1);

        var expense2 = new Expense
        {
            Amount = new Money(100),
            Date = today,
            Category = category,
            Store = "Hardware Store",
        };
        var expense2Added = await _transactionRepository.AddExpenseAsync(expense2);

        // Act
        var result = await _transactionRepository.GetAllExpensesAsync(
            today.Year, today.Month);
        var emptyResult = await _transactionRepository.GetAllExpensesAsync(
            today.Year - 1, today.Month);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(categoryAdded.IsSuccess, Is.True);

            Assert.That(expense1Added.IsSuccess, Is.True);
            Assert.That(expense2Added.IsSuccess, Is.True);
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));

            Assert.That(emptyResult, Is.Not.Null);
            Assert.That(emptyResult, Has.Count.EqualTo(0));
        });
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_GetAllIncomesAsync()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var category = new Category
        {
            Name = "Other",
        };
        var categoryAdded = await _categoryRepository.AddCategoryAsync(category);

        var income1 = new Income
        {
            Amount = new Money(50),
            Date = today,
            Category = category,
            Source = "Company",
        };
        var income1Added = await _transactionRepository.AddIncomeAsync(income1);

        var income2 = new Income
        {
            Amount = new Money(100),
            Date = today,
            Category = category,
            Source = "Freelance",
        };
        var income2Added = await _transactionRepository.AddIncomeAsync(income2);

        // Act
        var result = await _transactionRepository.GetAllIncomesAsync(
            today.Year, today.Month);
        var emptyResult = await _transactionRepository.GetAllIncomesAsync(
            today.Year - 1, today.Month);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(categoryAdded.IsSuccess, Is.True);

            Assert.That(income1Added.IsSuccess, Is.True);
            Assert.That(income2Added.IsSuccess, Is.True);
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));

            Assert.That(emptyResult, Is.Not.Null);
            Assert.That(emptyResult, Has.Count.EqualTo(0));
        });
    }
}
