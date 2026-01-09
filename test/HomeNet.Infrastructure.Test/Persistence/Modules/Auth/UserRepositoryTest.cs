using HomeNet.Core.Modules.Auth.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Auth;
using HomeNet.Infrastructure.Test.Containers;
using Npgsql;
using NUnit.Framework;
using SqlKata.Compilers;

namespace HomeNet.Infrastructure.Test.Persistence.Modules.Auth;

public class UserRepositoryTest
{
    private static readonly User _user1 = new User
    {
        Username = "testuser",
        PasswordHash = "hashedpassword",
        Role = UserRole.User,
    };
    private static readonly User _user2 = new User
    {
        Username = "adminuser",
        PasswordHash = "adminhashedpassword",
        Role = UserRole.Admin,
    };

    private UserRepository _userRepository;

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

        _userRepository = new UserRepository(db);
    }

    [TearDown]
    public async Task TearDown()
    {
        _userRepository.Dispose();

        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_AddUserAsync()
    {
        // Arrange

        // Act
        var result = await _userRepository.AddUserAsync(_user1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Error, Is.Null);
            Assert.That(_user1.Id, Is.EqualTo(1));
        });
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_GetUserByUsernameAsync()
    {
        // Arrange
        var user1Added = await _userRepository.AddUserAsync(_user1);
        var user2Added = await _userRepository.AddUserAsync(_user2);

        var invalidUsername = "invalid";

        // Act
        var user1FromDb = await _userRepository.GetUserByUsernameAsync(_user1.Username);
        var user2FromDb = await _userRepository.GetUserByUsernameAsync(_user2.Username);
        var invalidUserFromDb = await _userRepository.GetUserByUsernameAsync(invalidUsername);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(user1Added.IsSuccess, Is.True);
            Assert.That(user2Added.IsSuccess, Is.True);

            Assert.That(user1FromDb, Is.Not.Null);
            Assert.That(user1FromDb!.Id, Is.EqualTo(_user1.Id));
            Assert.That(user1FromDb.Username, Is.EqualTo(_user1.Username));

            Assert.That(user2FromDb, Is.Not.Null);
            Assert.That(user2FromDb!.Id, Is.EqualTo(_user2.Id));
            Assert.That(user2FromDb.Username, Is.EqualTo(_user2.Username));

            Assert.That(invalidUserFromDb, Is.Null);
        });
    }
}
