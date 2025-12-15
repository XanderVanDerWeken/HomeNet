using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Core.Modules.Finances.Queries;
using Moq;

namespace HomeNet.Core.Test.Modules.Finances.Queries;

public class CategoriesQueryHandlerTest
{
    private CategoriesQueryHandler _handler;

    private Mock<ICategoryRepository> _categoryRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();

        _handler = new CategoriesQueryHandler(
            _categoryRepositoryMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsList()
    {
        // Arrange
        var ct = new CancellationToken();
        
        var query = new CategoriesQuery();

        var category1 = new Category
        {
            Id = 1,
            Name = "Test Category",
        };

        var category2 = new Category
        {
            Id = 2,
            Name = "Another Category",
        };

        _categoryRepositoryMock
            .Setup(x => x.GetAllCategoriesAsync(ct))
            .ReturnsAsync([category1, category2]);

        // Act
        var result = await _handler.HandleAsync(query, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Not.Null);

            Assert.That(result.Value!, Has.Count.EqualTo(2));
            Assert.That(result.Value, Does.Contain(category1));
            Assert.That(result.Value, Does.Contain(category2));
        });

        _categoryRepositoryMock.Verify(
            r => r.GetAllCategoriesAsync(ct),
            Times.Once);
    }
}
