using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Errors;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Auth.Abstractions;

namespace HomeNet.Core.Modules.Auth.Commands;

public static class UnlinkPersonFromUser
{
    public sealed record Command : ICommand, IValidatable<Command>
    {
        public required string UserName { get; init; }

        public ValidationResult Validate()
            => new CommandValidator().Validate(this);
    }

    public sealed class CommandHandler : ICommandHandler<Command, Unit>
    {
        private readonly IUserRepository _userRepository;

        public CommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<Unit>> HandleAsync(
            Command command, CancellationToken cancellationToken = default)
        {
            var validationResult = command.Validate();

            if (!validationResult.IsValid)
            {
                return validationResult.ToFailure<Unit>();
            }

            var user = await _userRepository.GetUserByUsernameAsync(
                command.UserName);

            if (user is null)
            {
                return new NotFoundError("User", command.UserName).ToFailure<Unit>();
            }

            return await _userRepository.UpdatePersonLinkAsync(
                user.Id,
                null,
                cancellationToken); 
        }
    }

    private sealed class CommandValidator : BaseValidator<Command>
    {
        protected override void ValidateInternal(Command entity)
        {
            IsNotEmpty(entity.UserName, "UserName cannot be empty.");
        }
    }
}
