using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Finances.Commands;

public static class DeactivateFixedCost
{
    public sealed record Command : ICommand, IValidatable<Command>
    {
        public ValidationResult Validate()
            => new CommandValidator().Validate(this);
    }

    public sealed class CommandHandler : ICommandHandler<Command>
    {
        public Task<Result> HandleAsync(Command command, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    private sealed class CommandValidator : BaseValidator<Command>
    {
        protected override void ValidateInternal(Command entity)
        {
            throw new NotImplementedException();
        }
    }
}
