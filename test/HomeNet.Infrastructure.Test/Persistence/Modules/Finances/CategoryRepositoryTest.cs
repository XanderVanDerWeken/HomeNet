using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Finances;
using HomeNet.Infrastructure.Test.Containers;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using SqlKata.Compilers;

namespace HomeNet.Infrastructure.Test.Persistence.Modules.Finances;

public class CategoryRepositoryTest
{
    private static readonly Category _category1 = new Category
    {
        Name = "Test Category 1",
    };
    private static readonly Category _category2 = new Category
    {
        Name = "Test Category 2",
    };

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

        _categoryRepository = new CategoryRepository(
            NullLogger<CategoryRepository>.Instance, 
            db);
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
    public async Task Should_GetCategoryByNameAsync()
    {
        // Arrange
        var notFoundCategoryName = "NonExistentCategory";

        var addResult1 = await _categoryRepository.AddCategoryAsync(_category1);
        var addResult2 = await _categoryRepository.AddCategoryAsync(_category2);

        // Act
        var category1Result = await _categoryRepository.GetCategoryByNameAsync(_category1.Name);
        var category2Result = await _categoryRepository.GetCategoryByNameAsync(_category2.Name);
        var notFoundCategoryResult = await _categoryRepository.GetCategoryByNameAsync(notFoundCategoryName);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(addResult1.IsSuccess, Is.True);
            Assert.That(addResult2.IsSuccess, Is.True);

            Assert.That(category1Result, Is.Not.Null);
            Assert.That(category1Result?.Name, Is.EqualTo(_category1.Name));

            Assert.That(category2Result, Is.Not.Null);
            Assert.That(category2Result?.Name, Is.EqualTo(_category2.Name));

            Assert.That(notFoundCategoryResult, Is.Null);
        });
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_GetAllCategoriesAsync()
    {
        // Arrange
        var addResult1 = await _categoryRepository.AddCategoryAsync(_category1);
        var addResult2 = await _categoryRepository.AddCategoryAsync(_category2);

        // Act
        var result = await _categoryRepository.GetAllCategoriesAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(addResult1.IsSuccess, Is.True);
            Assert.That(addResult2.IsSuccess, Is.True);
            
            Assert.That(result, Has.Count.EqualTo(2));
        });
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_AddCategoryAsync()
    {
        // Act
        var result1 = await _categoryRepository.AddCategoryAsync(_category1);
        var result2 = await _categoryRepository.AddCategoryAsync(_category2);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result1.IsSuccess, Is.True);
            Assert.That(result1.Error, Is.Null);

            Assert.That(_category1.Id, Is.GreaterThan(0));

            Assert.That(result2.IsSuccess, Is.True);
            Assert.That(result2.Error, Is.Null);

            Assert.That(_category2.Id, Is.GreaterThan(0));
        });
    }
}
