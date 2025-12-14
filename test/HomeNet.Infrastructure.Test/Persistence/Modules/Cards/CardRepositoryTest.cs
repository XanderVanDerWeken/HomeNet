using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Cards;
using Npgsql;
using SqlKata.Compilers;
using Testcontainers.PostgreSql;

namespace HomeNet.Infrastructure.Test.Persistence.Modules.Cards;

public class CardRepositoryTest
{
    private CardRepository _cardRepository;

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

        _cardRepository = new CardRepository(db);
    }

    [TearDown]
    public async Task Teardown()
    {
        _cardRepository.Dispose();

        await _postgreSqlContainer.StopAsync();
        await _postgreSqlContainer.DisposeAsync();
    }

    [Test]
    [Explicit("Needs Docker running")]
    public void Should_AddCardAsync()
    {
        // Arrange

        // Act

        // Assert
        Assert.Pass();
    }

    [Test]
    [Explicit("Needs Docker running")]
    public void Should_GetCardByIdAsync()
    {
        // Arrange

        // Act

        // Assert
        Assert.Pass();
    }

    [Test]
    [Explicit("Needs Docker running")]
    public void Should_GetAllCardAsync()
    {
        // Arrange

        // Act

        // Assert
        Assert.Pass();
    }

    [Test]
    [Explicit("Needs Docker running")]
    public void Should_GetAllCardsWithExpiryBeforeAsync()
    {
        // Arrange

        // Act

        // Assert
        Assert.Pass();
    }

    [Test]
    [Explicit("Needs Docker running")]
    public void Should_UpdateCardAsync()
    {
        // Arrange

        // Act

        // Assert
        Assert.Pass();
    }

    [Test]
    [Explicit("Needs Docker running")]
    public void Should_RemoveCardAsync()
    {
        // Arrange

        // Act

        // Assert
        Assert.Pass();
    }
}
