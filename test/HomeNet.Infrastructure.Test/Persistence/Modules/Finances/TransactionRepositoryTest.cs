using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Finances;
using HomeNet.Infrastructure.Test.Containers;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using SqlKata.Compilers;

namespace HomeNet.Infrastructure.Test.Persistence.Modules.Finances;

public class TransactionRepositoryTest
{
    private TransactionRepository _transactionRepository;

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

        _transactionRepository = new TransactionRepository(
            NullLogger<TransactionRepository>.Instance, 
            db);
    }

    [TearDown]
    public async Task TearDown()
    {
        _transactionRepository.Dispose();

        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }

    [Test]
    public async Task Should_GetTransactionsWithCategoryIdAsync()
    {
        // Arrange

        // Act

        // Assert
    }

    [Test]
    public async Task Should_AddTransactionAsync()
    {
        // Arrange

        // Act

        // Assert
    }

    [Test]
    public async Task Should_GetMonthlySummaryAsync()
    {
        // Arrange

        // Act

        // Assert
    }
}
