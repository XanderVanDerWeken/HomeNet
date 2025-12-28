using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Cards.Commands;

public sealed class AddCardCommandValidator : IValidator<AddCardCommand>
{
    public ValidationResult Validate(AddCardCommand entity)
    {
        List<string> errors = [];

        if (string.IsNullOrWhiteSpace(entity.Name))
        {
            errors.Add("Name is required.");
        }

        if (entity.ExpirationDate <= DateOnly.FromDateTime(DateTime.UtcNow))
        {
            errors.Add("Expiration date must be in the future.");
        }

        return ValidationResult.FromErrors(errors);
    }
}
