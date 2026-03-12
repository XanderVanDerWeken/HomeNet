using HomeNet.Core.Common;
using HomeNet.Core.Common.Errors;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Commands;
using HomeNet.Core.Modules.Finances.Enums;
using HomeNet.Core.Modules.Finances.Models;
using Moq;

namespace HomeNet.Core.Test.Modules.Finances.Commands;

public class CreateTransactionTest
{
    private CreateTransaction.CommandHandler _handler;

    private Mock<ITransactionRepository> _transactionRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _transactionRepositoryMock = new Mock<ITransactionRepository>();

        _handler = new CreateTransaction.CommandHandler(
            _transactionRepositoryMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsSuccess()
    {
        // Arrange
        var ct = new CancellationToken();
        var command = new CreateTransaction.Command
        {
            CategoryId = 1,
            Amount = new Money(100),
            Type = TransactionType.Expense,
            Source = TransactionSource.Manual,
            Description = "Test transaction",
            Date = DateOnly.FromDateTime(DateTime.Now),
        };

        _transactionRepositoryMock
            .Setup(x => x.AddTransactionAsync(It.IsAny<Transaction>(), ct))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.HandleAsync(command, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Error, Is.Null);
        });

        _transactionRepositoryMock.Verify(
            x => x.AddTransactionAsync(
                It.Is<Transaction>(t =>
                    t.CategoryId == command.CategoryId &&
                    t.Amount == command.Amount &&
                    t.Type == command.Type &&
                    t.Source == command.Source &&
                    t.Description == command.Description &&
                    t.Date == command.Date),
                ct), 
            Times.Once);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_InvalidCommand()
    {
        // Arrange
        var commandCategoryIdEqual0 = new CreateTransaction.Command
        {
            CategoryId = 0,
            Amount = new Money(100),
            Type = TransactionType.Expense,
            Source = TransactionSource.Manual,
            Description = "Test transaction with invalid category ID",
            Date = DateOnly.FromDateTime(DateTime.Now),
        };
        var commandCategoryIdLessThan0 = new CreateTransaction.Command
        {
            CategoryId = -1,
            Amount = new Money(100),
            Type = TransactionType.Expense,
            Source = TransactionSource.Manual,
            Description = "Test transaction with invalid category ID",
            Date = DateOnly.FromDateTime(DateTime.Now),
        };

        var commandInvalidAmount = new CreateTransaction.Command
        {
            CategoryId = 1,
            Amount = new Money(-100), // Invalid amount
            Type = TransactionType.Expense,
            Source = TransactionSource.Manual,
            Description = "Test transaction with invalid amount",
            Date = DateOnly.FromDateTime(DateTime.Now),
        };

        // Act
        var resultCategoryIdEqual0 = await _handler.HandleAsync(commandCategoryIdEqual0);
        var resultCategoryIdLessThan0 = await _handler.HandleAsync(commandCategoryIdLessThan0);
        var resultInvalidAmount = await _handler.HandleAsync(commandInvalidAmount);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultCategoryIdEqual0.IsSuccess, Is.False);
            Assert.That(resultCategoryIdEqual0.Error, Is.Not.Null);
            Assert.That(resultCategoryIdEqual0.Error, Is.InstanceOf<ValidationError>());

            Assert.That(resultCategoryIdLessThan0.IsSuccess, Is.False);
            Assert.That(resultCategoryIdLessThan0.Error, Is.Not.Null);
            Assert.That(resultCategoryIdLessThan0.Error, Is.InstanceOf<ValidationError>());

            Assert.That(resultInvalidAmount.IsSuccess, Is.False);
            Assert.That(resultInvalidAmount.Error, Is.Not.Null);
            Assert.That(resultInvalidAmount.Error, Is.InstanceOf<ValidationError>());
        });

        _transactionRepositoryMock.Verify(
            x => x.AddTransactionAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }
}
