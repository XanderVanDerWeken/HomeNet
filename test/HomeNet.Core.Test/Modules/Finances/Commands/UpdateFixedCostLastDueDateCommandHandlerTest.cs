using HomeNet.Core.Common;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Commands;
using HomeNet.Core.Modules.Finances.Models;
using Microsoft.VisualBasic;
using Moq;

namespace HomeNet.Core.Test.Modules.Finances.Commands;

public class UpdateFixedCostLastDueDateCommandHandlerTest
{
    private UpdateFixedCostLastDueDateCommandHandler _handler;

    private Mock<IFixedCostRepository> _fixedCostRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _fixedCostRepositoryMock = new Mock<IFixedCostRepository>();

        _handler = new UpdateFixedCostLastDueDateCommandHandler(
            _fixedCostRepositoryMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsSuccess()
    {
        // Arrange
        var ct = new CancellationToken();

        var fixedCost = new FixedCost
        {
            Id = 1,
            Name = "TextCost",
            Amount = new Money(100),
            FirstDueDate = new DateOnly(2024, 1, 1),
            LastDueDate = null,
        };

        var command = new UpdateFixedCostLastDueDateCommand
        {
            FixedCostId = fixedCost.Id,
            LastDueDate = new DateOnly(2025, 1, 1),
        };

        _fixedCostRepositoryMock
            .Setup(x => x.GetFixedCostByIdAsync(command.FixedCostId, ct))
            .ReturnsAsync(fixedCost);
        
        _fixedCostRepositoryMock
            .Setup(x => x.UpdateFixedCostAsync(
                It.Is<FixedCost>(c => 
                    c.Id == fixedCost.Id &&
                    c.Name == fixedCost.Name &&
                    c.Amount == fixedCost.Amount &&
                    c.FirstDueDate == fixedCost.FirstDueDate &&
                    c.LastDueDate == command.LastDueDate), 
                ct))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.HandleAsync(command, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Error, Is.Null);
        });

        _fixedCostRepositoryMock.Verify(
            x => x.GetFixedCostByIdAsync(command.FixedCostId, ct), 
            Times.Once);
        
        _fixedCostRepositoryMock.Verify(
            x => x.UpdateFixedCostAsync(
                It.Is<FixedCost>(c => 
                    c.Id == fixedCost.Id &&
                    c.Name == fixedCost.Name &&
                    c.Amount == fixedCost.Amount &&
                    c.FirstDueDate == fixedCost.FirstDueDate &&
                    c.LastDueDate == command.LastDueDate), 
                ct),
            Times.Once);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_FixedCostNotFound()
    {
        // Arrange
        var ct = new CancellationToken();

        var invalidId = 999;

        var command = new UpdateFixedCostLastDueDateCommand
        {
            FixedCostId = invalidId,
            LastDueDate = new DateOnly(2025, 1, 1),
        };

        _fixedCostRepositoryMock
            .Setup(x => x.GetFixedCostByIdAsync(command.FixedCostId, ct))
            .ReturnsAsync((FixedCost?)null);

        // Act
        var result = await _handler.HandleAsync(command, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.Not.Null);
        });

        _fixedCostRepositoryMock.Verify(
            x => x.GetFixedCostByIdAsync(command.FixedCostId, ct), 
            Times.Once);
        
        _fixedCostRepositoryMock.Verify(
            x => x.UpdateFixedCostAsync(It.IsAny<FixedCost>(), ct),
            Times.Never);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_LastDateBeforeFirstDate()
    {
        // Arrange
        var ct = new CancellationToken();

        var fixedCost = new FixedCost
        {
            Id = 1,
            Name = "TextCost",
            Amount = new Money(100),
            FirstDueDate = new DateOnly(2024, 1, 1),
            LastDueDate = null,
        };

        var command = new UpdateFixedCostLastDueDateCommand
        {
            FixedCostId = fixedCost.Id,
            LastDueDate = fixedCost.FirstDueDate.AddDays(-7),
        };

        _fixedCostRepositoryMock
            .Setup(x => x.GetFixedCostByIdAsync(command.FixedCostId, ct))
            .ReturnsAsync(fixedCost);

        // Act
        var result = await _handler.HandleAsync(command, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.Not.Null);
        });

        _fixedCostRepositoryMock.Verify(
            x => x.GetFixedCostByIdAsync(command.FixedCostId, ct), 
            Times.Once);
        
        _fixedCostRepositoryMock.Verify(
            x => x.UpdateFixedCostAsync(It.IsAny<FixedCost>(), ct),
            Times.Never);
    }
}
