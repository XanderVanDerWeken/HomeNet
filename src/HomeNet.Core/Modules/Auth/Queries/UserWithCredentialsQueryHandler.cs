using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Auth.Abstractions;

namespace HomeNet.Core.Modules.Auth.Queries;

public sealed class UserWithCredentialsQueryHandler : IQueryHandler<UserWithCredentialsQuery, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;

    public UserWithCredentialsQueryHandler(
        IUserRepository userRepository,
        IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
    }

    public async Task<Result<bool>> HandleAsync(UserWithCredentialsQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = query.Validate();

        if (!validationResult.IsValid)
        {
            return validationResult.ToFailure<bool>();
        }

        var userWithUsername = await _userRepository
            .GetUserByUsernameAsync(query.UserName);
        
        if (userWithUsername == null)
        {
            return Result<bool>.Success(false);
        }
        
        var isPasswordValid = _passwordService.VerifyPassword(
            query.Password, userWithUsername.PasswordHash);

        return Result<bool>.Success(isPasswordValid);
    }
}
