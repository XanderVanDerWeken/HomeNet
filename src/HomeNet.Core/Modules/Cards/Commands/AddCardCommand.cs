using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Cards.Commands;

public sealed record AddCardCommand : ICommand, IValidatable<AddCardCommand>
{
    public required string Name { get; init; }

    public required DateOnly ExpirationDate { get; init; }

    public required int PersonId { get; init; }

    public ValidationResult Validate()
        => new AddCardCommandValidator().Validate(this);
}
