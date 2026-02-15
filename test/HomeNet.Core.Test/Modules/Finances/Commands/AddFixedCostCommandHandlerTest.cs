using HomeNet.Core.Common;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Commands;
using HomeNet.Core.Modules.Finances.Models;
using Moq;

namespace HomeNet.Core.Test.Modules.Finances.Commands;

public class AddFixedCostCommandHandlerTest
{
    private AddFixedCostCommandHandler _handler;

    private Mock<IFixedCostRepository> _fixedCostRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _fixedCostRepositoryMock = new Mock<IFixedCostRepository>();

        _handler = new AddFixedCostCommandHandler(
            _fixedCostRepositoryMock.Object);
    }

    [Test]
    public void Should_HandleAsync_ReturnsSuccess()
    {
        // Arrange
        var ct = new CancellationToken();

        var command1 = new AddFixedCostCommand
        {
            Name = "Fixed Cost without Last Due Date",
            Amount = new Money(100),
            FirstDueDate = new DateOnly(2025, 1, 1),
        };

        var command2 = new AddFixedCostCommand
        {
            Name = "Fixed Cost with Last Due Date",
            Amount = new Money(100),
            FirstDueDate = new DateOnly(2025, 1, 1),
            LastDueDate = new DateOnly(2025, 12, 31),
        };

        _fixedCostRepositoryMock
            .Setup(r => r.AddFixedCostAsync(
                It.Is<FixedCost>(fc =>
                    fc.Name == command1.Name &&
                    fc.Amount == command1.Amount &&
                    fc.FirstDueDate == command1.FirstDueDate &&
                    fc.LastDueDate == null),
                ct))
            .ReturnsAsync(Result.Success());

        _fixedCostRepositoryMock
            .Setup(r => r.AddFixedCostAsync(
                It.Is<FixedCost>(fc =>
                    fc.Name == command2.Name &&
                    fc.Amount == command2.Amount &&
                    fc.FirstDueDate == command2.FirstDueDate &&
                    fc.LastDueDate == command2.LastDueDate),
                ct))
            .ReturnsAsync(Result.Success());

        // Act
        var resultWithoutLastDueDate = _handler.HandleAsync(command1, ct);
        var resultWithLastDueDate = _handler.HandleAsync(command2, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultWithoutLastDueDate.Result.IsSuccess, Is.True);
            Assert.That(resultWithoutLastDueDate.Result.Error, Is.Null);

            Assert.That(resultWithLastDueDate.Result.IsSuccess, Is.True);
            Assert.That(resultWithLastDueDate.Result.Error, Is.Null);
        });

        _fixedCostRepositoryMock.Verify(
            r => r.AddFixedCostAsync(
                It.Is<FixedCost>(fc =>
                    fc.Name == command1.Name &&
                    fc.Amount == command1.Amount &&
                    fc.FirstDueDate == command1.FirstDueDate &&
                    fc.LastDueDate == null),
                ct),
            Times.Once);
        
        _fixedCostRepositoryMock.Verify(
            r => r.AddFixedCostAsync(
                It.Is<FixedCost>(fc =>
                    fc.Name == command2.Name &&
                    fc.Amount == command2.Amount &&
                    fc.FirstDueDate == command2.FirstDueDate &&
                    fc.LastDueDate == command2.LastDueDate),
                ct),
            Times.Once);
    }

    [Test]
    public void Should_HandleAsync_ReturnsFailure_InvalidCommand()
    {
        // Arrange
        var ct = new CancellationToken();

        var commandInvalidName = new AddFixedCostCommand
        {
            Name = string.Empty,
            Amount = new Money(100),
            FirstDueDate = new DateOnly(2025, 1, 1),
        };

        var commandInvalidDateInterval = new AddFixedCostCommand
        {
            Name = "Invalid Date Interval",
            Amount = new Money(100),
            FirstDueDate = new DateOnly(2025, 1, 1),
            LastDueDate = new DateOnly(2024, 12, 31),
        };

        // Act
        var resultInvalidName = _handler.HandleAsync(commandInvalidName, ct);
        var resultInvalidDateInterval = _handler.HandleAsync(commandInvalidDateInterval, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultInvalidName.Result.IsSuccess, Is.False);
            Assert.That(resultInvalidName.Result.Error, Is.Not.Null);

            Assert.That(resultInvalidDateInterval.Result.IsSuccess, Is.False);
            Assert.That(resultInvalidDateInterval.Result.Error, Is.Not.Null);
        });

        _fixedCostRepositoryMock.Verify(
            r => r.AddFixedCostAsync(
                It.IsAny<FixedCost>(),
                ct),
            Times.Never);
    }
}
