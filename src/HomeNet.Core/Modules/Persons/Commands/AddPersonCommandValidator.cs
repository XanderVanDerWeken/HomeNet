using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Persons.Commands;

public sealed class AddPersonCommandValidator : BaseValidator<AddPersonCommand>
{
    protected override void ValidateInternal(AddPersonCommand entity)
    {
        IsNotEmpty(entity.FirstName, "First Name is required");

        IsNotEmpty(entity.LastName, "Last Name is required");
    }
}
