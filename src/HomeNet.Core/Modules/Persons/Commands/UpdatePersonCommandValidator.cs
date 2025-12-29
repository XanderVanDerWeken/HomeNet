using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Persons.Commands;

public sealed class UpdatePersonCommandValidator : BaseValidator<UpdatePersonCommand>
{
    protected override void ValidateInternal(UpdatePersonCommand entity)
    {
        IsNullOrNotEmpty(entity.UpdatedFirstName, "Updated First Name cannot be empty");

        IsNullOrNotEmpty(entity.UpdatedLastName, "Updated Last Name cannot be empty");

        IsNullOrNotEmpty(entity.UpdatedAliasName, "Updated Alias Name cannot be empty");
    }

    private void IsNullOrNotEmpty(string? value, string errorMessage)
    {
        if (value != null)
        {
            IsNotEmpty(value, errorMessage);
        }
    }
}
