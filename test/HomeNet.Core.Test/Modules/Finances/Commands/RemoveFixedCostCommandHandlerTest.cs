using HomeNet.Core.Common;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Commands;
using Moq;

namespace HomeNet.Core.Test.Modules.Finances.Commands;

public class RemoveFixedCostCommandHandlerTest
{
    private RemoveFixedCostCommandHandler _handler;

    private Mock<IFixedCostRepository> _fixedCostRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _fixedCostRepositoryMock = new Mock<IFixedCostRepository>();

        _handler = new RemoveFixedCostCommandHandler(
            _fixedCostRepositoryMock.Object);
    }

    [Test]
    public void Should_HandleAsync_ReturnsSuccess()
    {
        // Arrange
        var ct = new CancellationToken();

        var fixedCostId = 1;
        var command = new RemoveFixedCostCommand
        {
            FixedCostId = fixedCostId,
        };

        _fixedCostRepositoryMock
            .Setup(r => r.RemoveFixedCostAsync(fixedCostId, ct))
            .ReturnsAsync(Result.Success());

        // Act
        var result = _handler.HandleAsync(command, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Result.IsSuccess, Is.True);
        });

        _fixedCostRepositoryMock.Verify(
            r => r.RemoveFixedCostAsync(fixedCostId, ct), 
            Times.Once);
    }
}
