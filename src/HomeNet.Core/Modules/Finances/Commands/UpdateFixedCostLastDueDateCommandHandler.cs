using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Finances.Abstractions;

namespace HomeNet.Core.Modules.Finances.Commands;

public sealed class UpdateFixedCostLastDueDateCommandHandler : ICommandHandler<UpdateFixedCostLastDueDateCommand>
{
    private readonly IFixedCostRepository _fixedCostRepository;

    public UpdateFixedCostLastDueDateCommandHandler(IFixedCostRepository fixedCostRepository)
    {
        _fixedCostRepository = fixedCostRepository;
    }

    public async Task<Result> HandleAsync(
        UpdateFixedCostLastDueDateCommand command, 
        CancellationToken cancellationToken = default)
    {
        var fixedCostToUpdate = await _fixedCostRepository.GetFixedCostByIdAsync(
            command.FixedCostId, 
            cancellationToken);
        
        if (fixedCostToUpdate == null)
        {
            return Result.Failure($"Fixed cost with ID {command.FixedCostId} not found.");
        }

        if (command.LastDueDate < fixedCostToUpdate.FirstDueDate)
        {
            return Result.Failure("Last due date cannot be earlier than the first due date.");
        }

        fixedCostToUpdate.LastDueDate = command.LastDueDate;
        return await _fixedCostRepository.UpdateFixedCostAsync(
            fixedCostToUpdate, 
            cancellationToken);
    }
}
