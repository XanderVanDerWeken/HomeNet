using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Auth.Abstractions;
using HomeNet.Core.Modules.Auth.Models;

namespace HomeNet.Core.Modules.Auth.Commands;

public sealed class AddUserCommandHandler : ICommandHandler<AddUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;

    public AddUserCommandHandler(
        IUserRepository userRepository, 
        IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
    }

    public Task<Result> HandleAsync(
        AddUserCommand command, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = command.Validate();

        if (!validationResult.IsValid)
        {
            return validationResult.ToFailure();
        }

        var hashedPassword = _passwordService.HashPassword(command.Password);

        var newUser = new User
        {
            UserName = command.UserName,
            PasswordHash = hashedPassword,
            Role = command.Role,
        };

        return _userRepository.AddUserAsync(
            newUser,
            cancellationToken);
    }
}
