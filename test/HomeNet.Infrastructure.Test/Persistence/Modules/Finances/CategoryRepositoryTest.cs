using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Finances;
using HomeNet.Infrastructure.Test.Containers;
using Npgsql;
using SqlKata.Compilers;

namespace HomeNet.Infrastructure.Test.Persistence.Modules.Finances;

public class CategoryRepositoryTest
{
    private CategoryRepository _categoryRepository;

    private HomenetPgContainer _dbContainer;

    [SetUp]
    public async Task Setup()
    {
        _dbContainer = new HomenetPgContainer();
        await _dbContainer.StartAsync();

        var connectionString = _dbContainer.GetConnectionString();

        var connection = new NpgsqlConnection(connectionString);
        var compiler = new PostgresCompiler();

        var db = new PostgresQueryFactory(connection, compiler);

        _categoryRepository = new CategoryRepository(db);
    }

    [TearDown]
    public async Task Teardown()
    {
        _categoryRepository.Dispose();

        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_AddCategoryAsync()
    {
        // Arrange
        var category = new Category
        {
            Name = "Groceries",
        };

        // Act
        var result = await _categoryRepository.AddCategoryAsync(category);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Error, Is.Null);
        });
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_GetAllCategoriesAsync()
    {
        // Arrange
        var category1 = new Category
        {
            Name = "Groceries",
        };

        var category2 = new Category
        {
            Name = "Utilities",
        };

        await _categoryRepository.AddCategoryAsync(category1);
        await _categoryRepository.AddCategoryAsync(category2);

        // Act
        var categories = await _categoryRepository.GetAllCategoriesAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(categories, Is.Not.Null);
            Assert.That(categories, Has.Count.EqualTo(2));
            Assert.That(categories.Any(c => c.Name == "Groceries"), Is.True);
            Assert.That(categories.Any(c => c.Name == "Utilities"), Is.True);
        });
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_GetCategoryAsync()
    {
        // Arrange
        var notFoundName = "not found";

        var category1 = new Category
        {
            Name = "Groceries",
        };

        var category2 = new Category
        {
            Name = "Utilities",
        };

        await _categoryRepository.AddCategoryAsync(category1);
        await _categoryRepository.AddCategoryAsync(category2);

        // Act
        var foundCategory1 = await _categoryRepository
            .GetCategoryByNameAsync(category1.Name);
        var foundCategory2 = await _categoryRepository
            .GetCategoryByNameAsync(category2.Name);
        var notFoundCategory = await _categoryRepository
            .GetCategoryByNameAsync(notFoundName);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(foundCategory1, Is.Not.Null);
            Assert.That(foundCategory1!.Name, Is.EqualTo(category1.Name));

            Assert.That(foundCategory2, Is.Not.Null);
            Assert.That(foundCategory2!.Name, Is.EqualTo(category2.Name));

            Assert.That(notFoundCategory, Is.Null);
        });
    }
}
