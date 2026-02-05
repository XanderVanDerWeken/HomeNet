using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Auth.Abstractions;

namespace HomeNet.Core.Modules.Auth.Commands;

public sealed class UnlinkPersonFromUserCommandHandler : ICommandHandler<UnlinkPersonFromUserCommand>
{
    private readonly IUserRepository _userRepository;

    public UnlinkPersonFromUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> HandleAsync(
        UnlinkPersonFromUserCommand command, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = command.Validate();

        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.ErrorMessage!);
        }

        var user = await _userRepository.GetUserByUsernameAsync(command.UserName);

        if (user is null)
        {
            return Result.Failure("User not found.");
        }

        return await _userRepository.UpdatePersonLinkAsync(
            user.Id,
            null,
            cancellationToken); 
    }
}
