using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Core.Modules.Finances.Queries;
using Moq;
using NUnit.Framework;

namespace HomeNet.Core.Test.Modules.Finances.Queries;

public class FixedCostsQueryHandlerTest
{
    private FixedCostsQueryHandler _handler;

    private Mock<IFixedCostRepository> _fixedCostRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _fixedCostRepositoryMock = new Mock<IFixedCostRepository>();

        _handler = new FixedCostsQueryHandler(
            _fixedCostRepositoryMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsList()
    {
        // Arrange
        var ct = new CancellationToken();

        var query = new FixedCostsQuery();

        var fixedCost1 = new FixedCost
        {
            Id = 1,
            Name = "Test Fixed Cost",
            Amount = new Money(100),
            FirstDueDate = new DateOnly(2024, 1, 1),
        };
        var fixedCost2 = new FixedCost
        {
            Id = 2,
            Name = "Other Fixed Cost",
            Amount = new Money(500),
            FirstDueDate = new DateOnly(2024, 2, 1),
        };

        _fixedCostRepositoryMock
            .Setup(x => x.GetAllFixedCostsAsync(ct))
            .ReturnsAsync([fixedCost1, fixedCost2]);

        // Act
        var result = await _handler.HandleAsync(query, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Not.Null);

            Assert.That(result.Value!, Has.Count.EqualTo(2));
            Assert.That(result.Value, Does.Contain(fixedCost1));
            Assert.That(result.Value, Does.Contain(fixedCost2));
        });

        _fixedCostRepositoryMock.Verify(
            x => x.GetAllFixedCostsAsync(ct),
            Times.Once);
    }
}
