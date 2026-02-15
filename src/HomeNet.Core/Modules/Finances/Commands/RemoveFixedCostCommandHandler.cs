using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Finances.Abstractions;

namespace HomeNet.Core.Modules.Finances.Commands;

public sealed class RemoveFixedCostCommandHandler : ICommandHandler<RemoveFixedCostCommand>
{
    private readonly IFixedCostRepository _fixedCostRepository;

    public RemoveFixedCostCommandHandler(IFixedCostRepository fixedCostRepository)
    {
        _fixedCostRepository = fixedCostRepository;
    }

    public Task<Result> HandleAsync(
        RemoveFixedCostCommand command, 
        CancellationToken cancellationToken = default)
    {
        return _fixedCostRepository.RemoveFixedCostAsync(
            command.FixedCostId, 
            cancellationToken);
    }
}
