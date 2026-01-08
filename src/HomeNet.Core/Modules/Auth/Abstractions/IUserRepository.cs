using HomeNet.Core.Modules.Auth.Models;

namespace HomeNet.Core.Modules.Auth.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(
        string username, 
        CancellationToken cancellationToken = default);

    Task CreateAsync(
        User user, 
        CancellationToken cancellationToken = default);
}
