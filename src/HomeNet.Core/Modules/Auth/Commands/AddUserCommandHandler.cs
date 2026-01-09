using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Auth.Abstractions;

namespace HomeNet.Core.Modules.Auth.Commands;

public sealed class AddUserCommandHandler : ICommandHandler<AddUserCommand>
{
    private readonly IUserRepository _userRepository;

    public AddUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> HandleAsync(
        AddUserCommand command, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = command.Validate();

        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.ErrorMessage!);
        }

        var newUser = new Models.User
        {
            Username = command.Username,
            PasswordHash = command.PasswordHash,
            Role = command.Role
        };

        await _userRepository.AddUserAsync(
            newUser, 
            cancellationToken);

        return Result.Success();
    }
}
