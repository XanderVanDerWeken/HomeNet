using HomeNet.Core.Common;
using HomeNet.Core.Common.Errors;
using HomeNet.Core.Modules.Auth.Abstractions;
using HomeNet.Core.Modules.Auth.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Auth.Entities;
using HomeNet.Infrastructure.Persistence.Modules.Auth.Extensions;
using Microsoft.Extensions.Logging;
using SqlKata;

namespace HomeNet.Infrastructure.Persistence.Modules.Auth;

public sealed class UserRepository : SqlKataRepository, IUserRepository
{
    private static readonly string TableName = "auth.users";

    private readonly ILogger _logger;

    public UserRepository(
        ILogger<UserRepository> logger, 
        PostgresQueryFactory db)
        : base(db)
    {
        _logger = logger;
    }
    
    public async Task<Result> AddUserAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Inserting new user with username: {Username}", user.UserName);
            var query = new Query(TableName).AsInsert(new
            {
                username = user.UserName,
                password_hash = user.PasswordHash,
                role = user.Role,
                person_id = user.PersonId,
            });

            var userId = await InsertAndReturnIdAsync(query);
            user.Id = userId;

            _logger.LogInformation("User inserted successfully with ID: {UserId}", userId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding user with username: {Username}", user.UserName);
            return new DatabaseError(TableName, ex).ToFailure();
        }
    }

    public async Task<User?> GetUserByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        var query = new Query(TableName)
            .Where("username", username);
        
        var entity = await FirstOrDefaultAsync<UserEntity>(
            query, 
            cancellationToken);
        
        return entity?.ToUser();
    }

    public async Task<Result> UpdatePersonLinkAsync(
        int userId, 
        int? personId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new Query(TableName)
                .Where("id", userId)
                .AsUpdate(new
                {
                    person_id = personId
                });

            var affectedRows = await ExecuteAsync(query, cancellationToken);
            return affectedRows > 0 
                ? Result.Success()
                : new NotFoundError("User", userId).ToFailure();
        }
        catch (Exception ex)
        {
            return new DatabaseError(TableName, ex).ToFailure();
        }
    }
}
