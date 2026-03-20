using HomeNet.Core.Common;
using HomeNet.Core.Modules.Finances.Enums;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Finances;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;
using HomeNet.Infrastructure.Test.Containers;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using SqlKata.Compilers;

namespace HomeNet.Infrastructure.Test.Persistence.Modules.Finances;

public class TransactionRepositoryTest
{
    private static readonly Category _category1 = new Category
    {
        Name = "Test Category 1",
    };
    private static readonly Category _category2 = new Category
    {
        Name = "Test Category 2",
    };

    private static readonly Transaction _transaction1 = new Transaction
    {
        Date = new DateOnly(2026, 1, 15),
        Amount = new Money(100.00m),
        Type = TransactionType.Income,
        Source = TransactionSource.Manual,
    };
    private static readonly Transaction _transaction2 = new Transaction
    {
        Date = new DateOnly(2026, 2, 15),
        Amount = new Money(100.00m),
        Type = TransactionType.Income,
        Source = TransactionSource.Manual,
    };
    private static readonly Transaction _transaction3 = new Transaction
    {
        Date = new DateOnly(2026, 2, 24),
        Amount = new Money(50.00m),
        Type = TransactionType.Expense,
        Source = TransactionSource.Manual,
    };

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

        _transactionRepository = new TransactionRepository(
            NullLogger<TransactionRepository>.Instance, 
            db);

        _categoryRepository = new CategoryRepository(
            NullLogger<CategoryRepository>.Instance, 
            db);
    }

    [TearDown]
    public async Task TearDown()
    {
        _categoryRepository.Dispose();

        _transactionRepository.Dispose();

        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_GetTransactionsWithCategoryIdAsync()
    {
        // Arrange
        var addCategory1Result = await _categoryRepository.AddCategoryAsync(_category1);
        var addCategory2Result = await _categoryRepository.AddCategoryAsync(_category2);

        _transaction1.CategoryId = _category1.Id;
        var addTransaction1Result = await _transactionRepository.AddTransactionAsync(_transaction1);

        _transaction2.CategoryId = _category1.Id;
        var addTransaction2Result = await _transactionRepository.AddTransactionAsync(_transaction2);

        _transaction3.CategoryId = _category2.Id;
        var addTransaction3Result = await _transactionRepository.AddTransactionAsync(_transaction3);

        var invalidCategoryId = -1;

        // Act
        var resultCategory1 = await _transactionRepository.GetTransactionsWithCategoryIdAsync(_category1.Id);
        var resultCategory2 = await _transactionRepository.GetTransactionsWithCategoryIdAsync(_category2.Id);
        var resultInvalidCategory = await _transactionRepository.GetTransactionsWithCategoryIdAsync(invalidCategoryId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(addCategory1Result.IsSuccess, Is.True);
            Assert.That(addCategory2Result.IsSuccess, Is.True);

            Assert.That(addTransaction1Result.IsSuccess, Is.True);
            Assert.That(addTransaction2Result.IsSuccess, Is.True);
            Assert.That(addTransaction3Result.IsSuccess, Is.True);

            Assert.That(resultCategory1, Has.Count.EqualTo(2));
            Assert.That(resultCategory1.FirstOrDefault(t => t.Id == _transaction1.Id), Is.Not.Null);
            Assert.That(resultCategory1.FirstOrDefault(t => t.Id == _transaction2.Id), Is.Not.Null);

            Assert.That(resultCategory2, Has.Count.EqualTo(1));
            Assert.That(resultCategory2.FirstOrDefault(t => t.Id == _transaction3.Id), Is.Not.Null);

            Assert.That(resultInvalidCategory, Is.Empty);
        });
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_AddTransactionAsync()
    {
        // Arrange
        var addCategory1Result = await _categoryRepository.AddCategoryAsync(_category1);
        var addCategory2Result = await _categoryRepository.AddCategoryAsync(_category2);

        // Act
        _transaction1.CategoryId = _category1.Id;
        var result1 = await _transactionRepository.AddTransactionAsync(_transaction1);

        _transaction2.CategoryId = _category1.Id;
        var result2 = await _transactionRepository.AddTransactionAsync(_transaction2);

        _transaction3.CategoryId = _category2.Id;
        var result3 = await _transactionRepository.AddTransactionAsync(_transaction3);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(addCategory1Result.IsSuccess, Is.True);
            Assert.That(addCategory2Result.IsSuccess, Is.True);

            Assert.That(result1.IsSuccess, Is.True);
            Assert.That(result1.Error, Is.Null);
            Assert.That(_transaction1.Id, Is.GreaterThan(0));

            Assert.That(result2.IsSuccess, Is.True);
            Assert.That(result2.Error, Is.Null);
            Assert.That(_transaction2.Id, Is.GreaterThan(0));

            Assert.That(result3.IsSuccess, Is.True);
            Assert.That(result3.Error, Is.Null);
            Assert.That(_transaction3.Id, Is.GreaterThan(0));
        });
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_GetMonthlySummaryAsync()
    {
        // Arrange
        var addCategory1Result = await _categoryRepository.AddCategoryAsync(_category1);
        var addCategory2Result = await _categoryRepository.AddCategoryAsync(_category2);

        _transaction1.CategoryId = _category1.Id;
        var addTransaction1Result = await _transactionRepository.AddTransactionAsync(_transaction1);

        _transaction2.CategoryId = _category1.Id;
        var addTransaction2Result = await _transactionRepository.AddTransactionAsync(_transaction2);

        _transaction3.CategoryId = _category2.Id;
        var addTransaction3Result = await _transactionRepository.AddTransactionAsync(_transaction3);

        // Act
        var resultJanuary = await _transactionRepository.GetMonthlySummaryAsync(2026, 1);
        var resultFebruary = await _transactionRepository.GetMonthlySummaryAsync(2026, 2);
        var resultMarch = await _transactionRepository.GetMonthlySummaryAsync(2026, 3);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(addCategory1Result.IsSuccess, Is.True);
            Assert.That(addCategory2Result.IsSuccess, Is.True);

            Assert.That(addTransaction1Result.IsSuccess, Is.True);
            Assert.That(addTransaction2Result.IsSuccess, Is.True);
            Assert.That(addTransaction3Result.IsSuccess, Is.True);

            // January
            Assert.That(resultJanuary.Year, Is.EqualTo(2026));
            Assert.That(resultJanuary.Month, Is.EqualTo(1));
            Assert.That(resultJanuary.TotalIncome, Is.EqualTo(new Money(100.00m)));
            Assert.That(resultJanuary.TotalExpenses, Is.EqualTo(Money.Zero));
            Assert.That(resultJanuary.Balance, Is.EqualTo(new Money(100.00m)));
            Assert.That(resultJanuary.ByCategory, Has.Count.EqualTo(1));

            var category1January = resultJanuary.ByCategory.FirstOrDefault(g => g.CategoryId == _category1.Id);
            Assert.That(category1January, Is.Not.Null);
            Assert.That(category1January!.CategoryName, Is.EqualTo(_category1.Name));
            Assert.That(category1January.TransactionType, Is.EqualTo(TransactionType.Income));
            Assert.That(category1January.TransactionCount, Is.EqualTo(1));
            Assert.That(category1January.TotalAmount, Is.EqualTo(new Money(100.00m)));

            // February
            Assert.That(resultFebruary.Year, Is.EqualTo(2026));
            Assert.That(resultFebruary.Month, Is.EqualTo(2));
            Assert.That(resultFebruary.TotalIncome, Is.EqualTo(new Money(100.00m)));
            Assert.That(resultFebruary.TotalExpenses, Is.EqualTo(new Money(50.00m)));
            Assert.That(resultFebruary.Balance, Is.EqualTo(new Money(50.00m)));
            Assert.That(resultFebruary.ByCategory, Has.Count.EqualTo(2));

            var category1February = resultFebruary.ByCategory.FirstOrDefault(g => g.CategoryId == _category1.Id);
            Assert.That(category1February, Is.Not.Null);
            Assert.That(category1February!.CategoryName, Is.EqualTo(_category1.Name));
            Assert.That(category1February.TransactionType, Is.EqualTo(TransactionType.Income));
            Assert.That(category1February.TransactionCount, Is.EqualTo(1));
            Assert.That(category1February.TotalAmount, Is.EqualTo(new Money(100.00m)));

            var category2February = resultFebruary.ByCategory.FirstOrDefault(g => g.CategoryId == _category2.Id);
            Assert.That(category2February, Is.Not.Null);
            Assert.That(category2February!.CategoryName, Is.EqualTo(_category2.Name));
            Assert.That(category2February.TransactionType, Is.EqualTo(TransactionType.Expense));
            Assert.That(category2February.TransactionCount, Is.EqualTo(1));
            Assert.That(category2February.TotalAmount, Is.EqualTo(new Money(50.00m)));

            // March
            Assert.That(resultMarch.Year, Is.EqualTo(2026));
            Assert.That(resultMarch.Month, Is.EqualTo(3));
            Assert.That(resultMarch.TotalIncome, Is.EqualTo(Money.Zero));
            Assert.That(resultMarch.TotalExpenses, Is.EqualTo(Money.Zero));
            Assert.That(resultMarch.Balance, Is.EqualTo(Money.Zero));
            Assert.That(resultMarch.ByCategory, Is.Empty);
        });
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_GetYearlySummaryAsync()
    {
        // Arrange
        var addCategory1Result = await _categoryRepository.AddCategoryAsync(_category1);
        var addCategory2Result = await _categoryRepository.AddCategoryAsync(_category2);

        _transaction1.CategoryId = _category1.Id;
        var addTransaction1Result = await _transactionRepository.AddTransactionAsync(_transaction1);

        _transaction2.CategoryId = _category1.Id;
        var addTransaction2Result = await _transactionRepository.AddTransactionAsync(_transaction2);

        _transaction3.CategoryId = _category2.Id;
        var addTransaction3Result = await _transactionRepository.AddTransactionAsync(_transaction3);

        // Act
        var result2026 = await _transactionRepository.GetYearlySummaryAsync(2026);
        var result2025 = await _transactionRepository.GetYearlySummaryAsync(2025);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(addCategory1Result.IsSuccess, Is.True);
            Assert.That(addCategory2Result.IsSuccess, Is.True);

            Assert.That(addTransaction1Result.IsSuccess, Is.True);
            Assert.That(addTransaction2Result.IsSuccess, Is.True);
            Assert.That(addTransaction3Result.IsSuccess, Is.True);

            // 2026
            Assert.That(result2026.Year, Is.EqualTo(2026));
            Assert.That(result2026.TotalIncome, Is.EqualTo(new Money(200.00m)));
            Assert.That(result2026.TotalExpenses, Is.EqualTo(new Money(50.00m)));
            Assert.That(result2026.Balance, Is.EqualTo(new Money(150.00m)));
            Assert.That(result2026.ByCategory, Has.Count.EqualTo(2));

            var category1Group = result2026.ByCategory.FirstOrDefault(g => g.CategoryId == _category1.Id);
            Assert.That(category1Group, Is.Not.Null);
            Assert.That(category1Group!.CategoryName, Is.EqualTo(_category1.Name));
            Assert.That(category1Group.TransactionType, Is.EqualTo(TransactionType.Income));
            Assert.That(category1Group.TransactionCount, Is.EqualTo(2));
            Assert.That(category1Group.TotalAmount, Is.EqualTo(new Money(200.00m)));

            var category2Group = result2026.ByCategory.FirstOrDefault(g => g.CategoryId == _category2.Id);
            Assert.That(category2Group, Is.Not.Null);
            Assert.That(category2Group!.CategoryName, Is.EqualTo(_category2.Name));
            Assert.That(category2Group.TransactionType, Is.EqualTo(TransactionType.Expense));
            Assert.That(category2Group.TransactionCount, Is.EqualTo(1));
            Assert.That(category2Group.TotalAmount, Is.EqualTo(new Money(50.00m)));

            // 2025
            Assert.That(result2025.Year, Is.EqualTo(2025));
            Assert.That(result2025.TotalIncome, Is.EqualTo(Money.Zero));
            Assert.That(result2025.TotalExpenses, Is.EqualTo(Money.Zero));
            Assert.That(result2025.Balance, Is.EqualTo(Money.Zero));
            Assert.That(result2025.ByCategory, Is.Empty);
        });
    }
}
