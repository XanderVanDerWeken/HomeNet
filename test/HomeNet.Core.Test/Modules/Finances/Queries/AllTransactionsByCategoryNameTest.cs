using HomeNet.Core.Common.Errors;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Enums;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Core.Modules.Finances.Queries;
using Moq;
using NUnit.Framework;

namespace HomeNet.Core.Test.Modules.Finances.Queries;

public class AllTransactionsByCategoryNameTest
{
    private AllTransactionsByCategoryName.QueryHandler _handler;

    private Mock<ITransactionRepository> _transactionRepositoryMock;
    private Mock<ICategoryRepository> _categoryRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();

        _handler = new AllTransactionsByCategoryName.QueryHandler(
            _transactionRepositoryMock.Object, 
            _categoryRepositoryMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsSuccess()
    {
        // Arrange
        var ct = new CancellationToken();

        var category = new Category
        {
            Id = 1,
            Name = "Test Category",
        };

        var transaction = new Transaction
        {
            Id = 1,
            CategoryId = category.Id,
            Amount = new Money(100),
            Date = DateOnly.FromDateTime(DateTime.Today),
            Source = TransactionSource.Manual,
            Type = TransactionType.Expense,
        };

        var query = new AllTransactionsByCategoryName.Query
        {
            CategoryName = category.Name,
        };

        _categoryRepositoryMock
            .Setup(x => x.GetCategoryByNameAsync(category.Name, ct))
            .ReturnsAsync(category);

        _transactionRepositoryMock
            .Setup(x => x.GetTransactionsWithCategoryIdAsync(category.Id, ct))
            .ReturnsAsync([transaction]);

        // Act
        var result = await _handler.HandleAsync(query, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value, Has.Count.EqualTo(1));
            Assert.That(result.Value, Does.Contain(transaction));
        });

        _categoryRepositoryMock.Verify(
            x => x.GetCategoryByNameAsync(category.Name, ct), 
            Times.Once);
        
        _transactionRepositoryMock.Verify(
            x => x.GetTransactionsWithCategoryIdAsync(category.Id, ct),
            Times.Once);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_CategoryNotFound()
    {
        // Arrange
        var ct = new CancellationToken();

        var category = new Category
        {
            Id = 1,
            Name = "Test Category",
        };

        var query = new AllTransactionsByCategoryName.Query
        {
            CategoryName = category.Name,
        };

        _categoryRepositoryMock
            .Setup(x => x.GetCategoryByNameAsync(category.Name, ct))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _handler.HandleAsync(query, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.InstanceOf<NotFoundError>());
        });

        _categoryRepositoryMock.Verify(
            x => x.GetCategoryByNameAsync(category.Name, ct), 
            Times.Once);
        
        _transactionRepositoryMock.Verify(
            x => x.GetTransactionsWithCategoryIdAsync(It.IsAny<int>(), ct),
            Times.Never);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_InvalidQuery()
    {
        // Arrange
        var ct = new CancellationToken();

        var queryInvalidCategoryName = new AllTransactionsByCategoryName.Query
        {
            CategoryName = string.Empty,
        };

        // Act
        var resultInvalidCategoryName = await _handler.HandleAsync(queryInvalidCategoryName, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultInvalidCategoryName.IsSuccess, Is.False);
            Assert.That(resultInvalidCategoryName.Error, Is.InstanceOf<ValidationError>());
        });

        _categoryRepositoryMock.Verify(
            x => x.GetCategoryByNameAsync(It.IsAny<string>(), ct), 
            Times.Never);
        
        _transactionRepositoryMock.Verify(
            x => x.GetTransactionsWithCategoryIdAsync(It.IsAny<int>(), ct),
            Times.Never);
    }
}
