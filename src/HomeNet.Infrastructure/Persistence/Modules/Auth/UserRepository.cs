using HomeNet.Core.Common;
using HomeNet.Core.Common.Errors;
using HomeNet.Core.Modules.Auth.Abstractions;
using HomeNet.Core.Modules.Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeNet.Infrastructure.Persistence.Modules.Auth;

public sealed class UserRepository : IUserRepository
{
    private readonly UserDbContext _dbContext;

    public UserRepository(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<User>> AddUserAsync(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(user);
    }

    public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == username, cancellationToken);
    }

    public async Task<Result<Unit>> UpdatePersonLinkAsync(int userId, int? personId, CancellationToken cancellationToken = default)
    {
        var affectedRows = await _dbContext.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(update => update
                .SetProperty(u => u.PersonId, personId),
                cancellationToken);
        
        return affectedRows > 0
            ? Result.Success(Unit.Value)
            : new NotFoundError(nameof(User), userId).ToFailure<Unit>();
    }
}
