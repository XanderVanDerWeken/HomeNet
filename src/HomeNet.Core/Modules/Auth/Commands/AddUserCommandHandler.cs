using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Auth.Abstractions;
using HomeNet.Core.Modules.Auth.Models;

namespace HomeNet.Core.Modules.Auth.Commands;

public sealed class AddUserCommandHandler : ICommandHandler<AddUserCommand>
{
    private readonly IUserRepository _userRepository;

    public AddUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<Result> HandleAsync(
        AddUserCommand command, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = command.Validate();

        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.ErrorMessage!);
        }

        var newUser = new User
        {
            UserName = command.UserName,
            PasswordHash = command.PasswordHash,
            Role = command.Role,
        };

        return _userRepository.AddUserAsync(
            newUser,
            cancellationToken);
    }
}
