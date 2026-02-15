using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Commands;

public sealed class AddFixedCostCommandHandler : ICommandHandler<AddFixedCostCommand>
{
    private readonly IFixedCostRepository _fixedCostRepository;

    public AddFixedCostCommandHandler(IFixedCostRepository fixedCostRepository)
    {
        _fixedCostRepository = fixedCostRepository;
    }

    public Task<Result> HandleAsync(
        AddFixedCostCommand command, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = command.Validate();

        if (!validationResult.IsValid)
        {
            return validationResult.ToFailure();
        }

        var newFixedCost = new FixedCost
        {
            Name = command.Name,
            Amount = command.Amount,
            FirstDueDate = command.FirstDueDate,
            LastDueDate = command.LastDueDate,
        };

        return _fixedCostRepository.AddFixedCostAsync(
            newFixedCost, 
            cancellationToken);
    }
}
