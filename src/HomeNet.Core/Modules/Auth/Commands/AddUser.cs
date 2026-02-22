using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Auth.Abstractions;
using HomeNet.Core.Modules.Auth.Models;

namespace HomeNet.Core.Modules.Auth.Commands;

public static class AddUser
{
    public sealed record Command : ICommand, IValidatable<Command>
    {
        public required string UserName { get; init; }

        public required string Password { get; init; }

        public string Role { get; init; } = "User";

        public ValidationResult Validate()
            => new CommandValidator().Validate(this);
    }

    public sealed class CommandHandler : ICommandHandler<Command>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;

        public CommandHandler(
            IUserRepository userRepository, 
            IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
        }

        public Task<Result> HandleAsync(
            Command command, CancellationToken cancellationToken = default)
        {
            var validationResult = command.Validate();

            if (!validationResult.IsValid)
            {
                return validationResult.ToFailure();
            }

            var hashedPassword = _passwordService.HashPassword(
                command.Password);

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

    private sealed class CommandValidator : BaseValidator<Command>
    {
        protected override void ValidateInternal(Command entity)
        {
            IsNotEmpty(entity.UserName, "User name must not be empty.");
        
            IsNotEmpty(entity.Password, "Password must not be empty.");
            
            if (entity.Role != "User" && entity.Role != "Admin")
            {
                Errors.Add("Role must be either 'User' or 'Admin'.");
            }
        }
    }
}
