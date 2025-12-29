using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Cards.Commands;

public sealed class AddCardCommandValidator : BaseValidator<AddCardCommand>
{
    protected override void ValidateInternal(AddCardCommand entity)
    {
        IsNotEmpty(entity.Name, "Name is required");

        if (entity.ExpirationDate <= DateOnly.FromDateTime(DateTime.UtcNow))
        {
            Errors.Add("Expiration date must be in the future.");
        }
    }
}
