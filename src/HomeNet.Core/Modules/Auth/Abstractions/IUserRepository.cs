using HomeNet.Core.Common;
using HomeNet.Core.Modules.Auth.Models;

namespace HomeNet.Core.Modules.Auth.Abstractions;

public interface IUserRepository
{
    public Task<Result<User>> AddUserAsync(
        User user,
        CancellationToken cancellationToken = default);
    
    public Task<User?> GetUserByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default);
    
    public Task<Result<Unit>> UpdatePersonLinkAsync(
        int userId, 
        int? personId, 
        CancellationToken cancellationToken = default);
}
