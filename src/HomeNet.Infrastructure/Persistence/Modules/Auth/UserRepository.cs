using HomeNet.Core.Common;
using HomeNet.Core.Modules.Auth.Abstractions;
using HomeNet.Core.Modules.Auth.Extensions;
using HomeNet.Core.Modules.Auth.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Auth.Entities;
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
                username = user.Username,
                password = user.PasswordHash,
                role = user.Role.ToRoleString(),
            });

            var newUserId = await InsertAndReturnIdAsync(query);
            user.Id = newUserId;

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while creating the user: {ex.Message}");
        }
    }

    public async Task<User?> GetUserByUsername(
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
}
