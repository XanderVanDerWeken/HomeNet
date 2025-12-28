using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Finances.Commands;

public sealed record AddCategoryCommand : ICommand, IValidatable<AddCategoryCommand>
{
    public required string Name { get; init; }

    public ValidationResult Validate()
        => new AddCategoryCommandValidator().Validate(this);
}
