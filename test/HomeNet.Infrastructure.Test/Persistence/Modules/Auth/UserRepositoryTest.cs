using HomeNet.Core.Modules.Auth.Models;
using HomeNet.Core.Modules.Persons.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Auth;
using HomeNet.Infrastructure.Persistence.Modules.Persons;
using HomeNet.Infrastructure.Test.Containers;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using SqlKata.Compilers;

namespace HomeNet.Infrastructure.Test.Persistence.Modules.Auth;

public class UserRepositoryTest
{
    private static readonly User _user1 = new User
    {
        UserName = "TestUser1",
        PasswordHash = "hashedpassword1",
        Role = "User",
    };
    private static readonly User _user2 = new User
    {
        UserName = "admin",
        PasswordHash = "hashedpassword2",
        Role = "Admin",
    };

    private UserRepository _userRepository;
    private PersonRepository _personRepository;

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

        _personRepository = new PersonRepository(db);

        _userRepository = new UserRepository(
            NullLogger<UserRepository>.Instance, 
            db);
    }

    [TearDown]
    public async Task Teardown()
    {
        _userRepository.Dispose();
        _personRepository.Dispose();

        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_AddUserAsync()
    {
        // Arrange
        var invalidUser = new User
        {
            UserName = "invalidUser",
            PasswordHash = "hashedpassword3",
            Role = "SuperUser", // Invalid role
        };

        // Act
        var resultUser1 = await _userRepository.AddUserAsync(_user1);
        var resultUser2 = await _userRepository.AddUserAsync(_user2);
        var resultInvalidRole = await _userRepository.AddUserAsync(invalidUser);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultUser1.IsSuccess, Is.True);
            Assert.That(resultUser1.Error, Is.Null);

            Assert.That(resultUser2.IsSuccess, Is.True);
            Assert.That(resultUser2.Error, Is.Null);

            Assert.That(resultInvalidRole.IsSuccess, Is.False);
            Assert.That(resultInvalidRole.Error, Is.Not.Null);
        });
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_GetUserByUsernameAsync()
    {
        // Arrange
        var notFoundUsername = "nonExistentUser";

        var addResult1 = await _userRepository.AddUserAsync(_user1);
        var addResult2 = await _userRepository.AddUserAsync(_user2);

        // Act
        var user1 = await _userRepository.GetUserByUsernameAsync(_user1.UserName);
        var user2 = await _userRepository.GetUserByUsernameAsync(_user2.UserName);
        var notFoundUser = await _userRepository.GetUserByUsernameAsync(notFoundUsername);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(addResult1.IsSuccess, Is.True);
            Assert.That(addResult2.IsSuccess, Is.True);

            Assert.That(user1, Is.Not.Null);
            Assert.That(user1?.UserName, Is.EqualTo(_user1.UserName));
            Assert.That(user1?.PasswordHash, Is.EqualTo(_user1.PasswordHash));
            Assert.That(user1?.Role, Is.EqualTo(_user1.Role));

            Assert.That(user2, Is.Not.Null);
            Assert.That(user2?.UserName, Is.EqualTo(_user2.UserName));
            Assert.That(user2?.PasswordHash, Is.EqualTo(_user2.PasswordHash));
            Assert.That(user2?.Role, Is.EqualTo(_user2.Role));

            Assert.That(notFoundUser, Is.Null);
        });
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_UpdatePersonLinkAsync()
    {
        // Arrange
        var invalidUserId = 999;

        var person = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            AliasName = "JD",
            IsInactive = false,
        };

        var personAddedResult = await _personRepository.AddPersonAsync(person);

        var addResult1 = await _userRepository.AddUserAsync(_user1);
        var addResult2 = await _userRepository.AddUserAsync(_user2);
        
        // Act
        var resultAddedLink = await _userRepository.UpdatePersonLinkAsync(_user1.Id, person.Id);
        var resultRemovedLink = await _userRepository.UpdatePersonLinkAsync(_user1.Id, null);
        var resultInvalidUserId = await _userRepository.UpdatePersonLinkAsync(invalidUserId, person.Id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(personAddedResult.IsSuccess, Is.True);
            Assert.That(addResult1.IsSuccess, Is.True);
            Assert.That(addResult2.IsSuccess, Is.True);

            Assert.That(resultAddedLink.IsSuccess, Is.True);
            Assert.That(resultAddedLink.Error, Is.Null);

            Assert.That(resultRemovedLink.IsSuccess, Is.True);
            Assert.That(resultRemovedLink.Error, Is.Null);

            Assert.That(resultInvalidUserId.IsSuccess, Is.False);
            Assert.That(resultInvalidUserId.Error, Is.Not.Null);
        });
    }
}
