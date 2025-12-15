using HomeNet.Core.Common;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Commands;
using HomeNet.Core.Modules.Finances.Models;
using Moq;

namespace HomeNet.Core.Test.Modules.Finances.Commands;

public class AddCategoryCommandHandlerTest
{
    private AddCategoryCommandHandler _handler;

    private Mock<ICategoryRepository> _categoryRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();

        _handler = new AddCategoryCommandHandler(
            _categoryRepositoryMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsSuccess()
    {
        // Arrange
        var ct = new CancellationToken();

        var command = new AddCategoryCommand
        {
            Name = "Test Category",
        };

        _categoryRepositoryMock
            .Setup(r => r.AddCategoryAsync(
                It.Is<Category>(c => c.Name == command.Name), 
                ct))
            .ReturnsAsync(Result.Success());

        // Act
        var validResult = await _handler.HandleAsync(command, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(validResult.IsSuccess, Is.True);
            Assert.That(validResult.Error, Is.Null);
        });

        _categoryRepositoryMock.Verify(
            r => r.AddCategoryAsync(
                It.Is<Category>(c =>
                    c.Name == command.Name),
                ct),
            Times.Once);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_InvalidCommand()
    {
        // Arrange
        var ct = new CancellationToken();

        var command = new AddCategoryCommand
        {
            Name = string.Empty,
        };

        // Act
        var result = await _handler.HandleAsync(command, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.Not.Null);
        });

        _categoryRepositoryMock.Verify(
            r => r.AddCategoryAsync(
                It.IsAny<Category>(),
                ct),
            Times.Never);
    }
}
