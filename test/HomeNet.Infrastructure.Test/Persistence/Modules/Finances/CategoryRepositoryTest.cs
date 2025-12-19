using DotNet.Testcontainers.Builders;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Finances;
using HomeNet.Infrastructure.Test.Containers;
using Npgsql;
using NUnit.Framework;
using SqlKata.Compilers;
using Testcontainers.PostgreSql;

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
}
