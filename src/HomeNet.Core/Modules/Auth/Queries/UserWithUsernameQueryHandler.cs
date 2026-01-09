using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Auth.Abstractions;
using HomeNet.Core.Modules.Auth.Models;

namespace HomeNet.Core.Modules.Auth.Queries;

public sealed class UserWithUsernameQueryHandler : IQueryHandler<UserWithUsernameQuery, User?>
{
    private readonly IUserRepository _userRepository;

    public UserWithUsernameQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<User?>> HandleAsync(
        UserWithUsernameQuery query, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = query.Validate();

        if (!validationResult.IsValid)
        {
            return Result<User?>.Failure(validationResult.ErrorMessage!);
        }

        var user = await _userRepository.GetUserByUsernameAsync(
            query.Username, 
            cancellationToken);

        return Result<User?>.Success(user);
    }
}
