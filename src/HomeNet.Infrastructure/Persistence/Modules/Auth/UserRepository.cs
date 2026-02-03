using System;
using HomeNet.Core.Common;
using HomeNet.Core.Modules.Auth.Abstractions;
using HomeNet.Core.Modules.Auth.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Auth.Extensions;
using SqlKata;

namespace HomeNet.Infrastructure.Persistence.Modules.Auth;

public sealed class UserRepository : SqlKataRepository, IUserRepository
{
    private static readonly string TableName = "auth.users";

    public UserRepository(PostgresQueryFactory db)
        : base(db)
    {
    }
    
    public async Task<Result> AddUserAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new Query(TableName).AsInsert(new
            {
                username = user.UserName,
                password_hash = user.PasswordHash,
                role = user.Role,
            });

            var userId = await InsertAndReturnIdAsync(query);
            user.Id = userId;

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while adding the user: {ex.Message}");
        }
    }

    public async Task<User?> GetUserByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        var query = new Query(TableName)
            .Where("username", username);
        
        var entity = await FirstOrDefaultAsync<Entities.UserEntity>(
            query, 
            cancellationToken);
        
        return entity?.ToUser();
    }
}
