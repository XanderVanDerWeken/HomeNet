using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Cards.Commands;

public sealed record AddCardCommand : ICommand, IValidatable<AddCardCommand>
{
    public required string Name { get; init; }

    public required DateTime ExpirationDate { get; init; }

    public IValidator<AddCardCommand> GetValidator() 
        => new AddCardCommandValidator();
}
