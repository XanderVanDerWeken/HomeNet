using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Finances;
using Npgsql;
using NUnit.Framework;
using SqlKata.Compilers;
using Testcontainers.PostgreSql;

namespace HomeNet.Infrastructure.Test.Persistence.Modules.Finances;

public class CategoryRepositoryTest
{
    private CategoryRepository _categoryRepository;

    private PostgreSqlContainer _postgreSqlContainer;

    [SetUp]
    public async Task Setup()
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:17")
            .WithDatabase("homenet_test")
            .WithUsername("homenet_user")
            .WithPassword("homenet_password")
            .Build();

        await _postgreSqlContainer.StartAsync();

        var connectionString = _postgreSqlContainer.GetConnectionString();

        var connection = new NpgsqlConnection(connectionString);
        var compiler = new PostgresCompiler();

        var db = new PostgresQueryFactory(connection, compiler);

        _categoryRepository = new CategoryRepository(db);
    }

    [TearDown]
    public async Task Teardown()
    {
        _categoryRepository.Dispose();

        await _postgreSqlContainer.StopAsync();
        await _postgreSqlContainer.DisposeAsync();
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
