using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Finances.Abstractions;

namespace HomeNet.Core.Modules.Finances.Commands;

public static class DeactivateFixedCost
{
    public sealed record Command : ICommand
    {
        public int FixedCostId { get; init; }
    }

    public sealed class CommandHandler : ICommandHandler<Command, Unit>
    {
        private readonly IFixedCostRepository _fixedCostRepository;

        public CommandHandler(IFixedCostRepository fixedCostRepository)
        {
            _fixedCostRepository = fixedCostRepository;
        }

        public Task<Result<Unit>> HandleAsync(
            Command command, CancellationToken cancellationToken = default)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            return _fixedCostRepository.DeactivateLastVersionAsync(
                command.FixedCostId, today, cancellationToken);
        }
    }
}
