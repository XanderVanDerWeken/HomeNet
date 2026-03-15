using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Core.Modules.Finances.Queries;
using Moq;

namespace HomeNet.Core.Test.Modules.Finances.Queries;

public class AllCategoriesTest
{
    private AllCategories.QueryHandler _handler;

    private Mock<ICategoryRepository> _categoryRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();

        _handler = new AllCategories.QueryHandler(
            _categoryRepositoryMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsSuccess()
    {
        // Arrange
        var ct = new CancellationToken();

        var query = new AllCategories.Query();

        var category = new Category
        {
            Id = 1,
            Name = "Test Category",
        };

        _categoryRepositoryMock
            .Setup(x => x.GetAllCategoriesAsync(ct))
            .ReturnsAsync([category]);

        // Act
        var result = await _handler.HandleAsync(query, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value, Has.Count.EqualTo(1));
            Assert.That(result.Value, Does.Contain(category));
        });

        _categoryRepositoryMock.Verify(
            x => x.GetAllCategoriesAsync(ct), 
            Times.Once);
    }
}
