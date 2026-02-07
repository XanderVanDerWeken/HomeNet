using HomeNet.Core.Common;
using HomeNet.Core.Modules.Auth.Models;

namespace HomeNet.Core.Modules.Auth.Abstractions;

public interface IUserRepository
{
    Task<Result> AddUserAsync(
        User user,
        CancellationToken cancellationToken = default);
    
    Task<User?> GetUserByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default);
    
    Task<Result> UpdatePersonLinkAsync(
        int userId, 
        int? personId, 
        CancellationToken cancellationToken = default);
}
