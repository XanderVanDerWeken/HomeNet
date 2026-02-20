using HomeNet.Core.Common.Errors;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Commands;
using HomeNet.Core.Modules.Finances.Models;
using Moq;

namespace HomeNet.Core.Test.Modules.Finances.Commands;

public class AddExpenseCommandHandlerTest
{
    private AddExpenseCommandHandler _handler;

    private Mock<ITransactionRepository> _transactionRepositoryMock;
    private Mock<ICategoryRepository> _categoryRepositoryMock;

    private static readonly Money _amount = new Money(72.99m);
    private static readonly DateOnly _date = new DateOnly(2025, 1, 1);
    private static readonly string _categoryName = "Groceries";
    private static readonly string _storeName = "Grocery Store";

    [SetUp]
    public void Setup()
    {
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();

        _handler = new AddExpenseCommandHandler(
            _transactionRepositoryMock.Object,
            _categoryRepositoryMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsSuccess()
    {
        // Arrange
        var ct = new CancellationToken();

        var command = new AddExpenseCommand
        {
            Amount = _amount,
            Date = _date,
            CategoryName = _categoryName,
            StoreName = _storeName,
        };

        var category = new Category
        {
            Id = 1,
            Name = _categoryName,
        };

        _categoryRepositoryMock
            .Setup(r => r.GetCategoryByNameAsync(
                _categoryName,
                ct))
            .ReturnsAsync(category);
        
        _transactionRepositoryMock
            .Setup(r => r.AddExpenseAsync(
                It.Is<Expense>(e =>
                    e.Amount == _amount &&
                    e.Date == _date &&
                    e.Category == category &&
                    e.Store == _storeName),
                ct))
            .ReturnsAsync(Core.Common.Result.Success());

        // Act
        var result = await _handler.HandleAsync(command, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Error, Is.Null);
        });

        _categoryRepositoryMock.Verify(
            r => r.GetCategoryByNameAsync(
                _categoryName,
                ct),
            Times.Once);
        
        _transactionRepositoryMock.Verify(
            r => r.AddExpenseAsync(
                It.Is<Expense>(e =>
                    e.Amount == _amount &&
                    e.Date == _date &&
                    e.Category == category &&
                    e.Store == _storeName),
                ct),
            Times.Once);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_InvalidCommand()
    {
        // Arrange
        var ct = new CancellationToken();
        
        var commandInvalidCategory = new AddExpenseCommand
        {
            Amount = _amount,
            Date = _date,
            CategoryName = string.Empty,
            StoreName = _storeName,
        };

        var commandInvalidStore = new AddExpenseCommand
        {
            Amount = _amount,
            Date = _date,
            CategoryName = _categoryName,
            StoreName = string.Empty,
        };

        // Act
        var resultInvalidCategory = await _handler
            .HandleAsync(commandInvalidCategory, ct);
        
        var resultInvalidStore = await _handler
            .HandleAsync(commandInvalidStore, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultInvalidCategory.IsSuccess, Is.False);
            Assert.That(resultInvalidCategory.Error, Is.Not.Null);
            
            Assert.That(resultInvalidStore.IsSuccess, Is.False);
            Assert.That(resultInvalidStore.Error, Is.Not.Null);
        });

        _categoryRepositoryMock.Verify(
            r => r.GetCategoryByNameAsync(
                _categoryName,
                ct),
            Times.Never);
        
        _transactionRepositoryMock.Verify(
            r => r.AddExpenseAsync(
                It.IsAny<Expense>(),
                ct),
            Times.Never);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_CategoryNotFound()
    {
        // Arrange
        var ct = new CancellationToken();

        var command = new AddExpenseCommand
        {
            Amount = _amount,
            Date = _date,
            CategoryName = _categoryName,
            StoreName = _storeName,
        };

        _categoryRepositoryMock
            .Setup(r => r.GetCategoryByNameAsync(
                _categoryName,
                ct));

        // Act
        var result = await _handler.HandleAsync(command, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.Not.Null);
            Assert.That(result.Error, Is.InstanceOf<NotFoundError>());
        });

        _categoryRepositoryMock.Verify(
            r => r.GetCategoryByNameAsync(
                _categoryName,
                ct),
            Times.Once);
        
        _transactionRepositoryMock.Verify(
            r => r.AddExpenseAsync(
                It.IsAny<Expense>(),
                ct),
            Times.Never);
    }
}
