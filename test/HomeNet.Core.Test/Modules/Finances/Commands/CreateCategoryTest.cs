using HomeNet.Core.Common;
using HomeNet.Core.Common.Errors;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Commands;
using HomeNet.Core.Modules.Finances.Models;
using Moq;

namespace HomeNet.Core.Test.Modules.Finances.Commands;

public class CreateCategoryTest
{
    private CreateCategory.CommandHandler _handler;

    private Mock<ICategoryRepository> _categoryRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();

        _handler = new CreateCategory.CommandHandler(
            _categoryRepositoryMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsSuccess()
    {
        // Arrange
        var ct = new CancellationToken();
        var command = new CreateCategory.Command
        {
            Name = "Test Category",
        };

        _categoryRepositoryMock
            .Setup(x => x.AddCategoryAsync(It.IsAny<Category>(), ct))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.HandleAsync(command, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Error, Is.Null);
        });

        _categoryRepositoryMock.Verify(
            x => x.AddCategoryAsync(
                It.Is<Category>(c =>
                    c.Name == command.Name),
                ct),
            Times.Once());
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_InvalidCommand()
    {
        // Arrange
        var commandInvalidName = new CreateCategory.Command
        {
            Name = string.Empty,
        };

        // Act
        var resultInvalidName = await _handler.HandleAsync(commandInvalidName);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultInvalidName.IsSuccess, Is.False);
            Assert.That(resultInvalidName.Error, Is.Not.Null);
            Assert.That(resultInvalidName.Error, Is.InstanceOf<ValidationError>());
        });

        _categoryRepositoryMock.Verify(
            x => x.AddCategoryAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()),
            Times.Never());
    }
}
