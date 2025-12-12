using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Test.Common.Validation;

public class ValidatorTest
{
    [Test]
    public void Should_Validate()
    {
        // Arrange
        var validEntity = new DummyEntity
        {
            Value = "ValidValue"
        };

        var invalidEntity = new DummyEntity
        {
            Value = null
        };

        // Act
        var validResult = validEntity.GetValidator().Validate(validEntity);
        var invalidResult = invalidEntity.GetValidator().Validate(invalidEntity);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(validResult.IsValid, Is.True);
            Assert.That(validResult.Errors, Is.Empty);

            Assert.That(invalidResult.IsValid, Is.False);
            Assert.That(invalidResult.Errors, Is.Not.Empty);
            Assert.That(invalidResult.Errors, Has.Count.EqualTo(1));
            Assert.That(invalidResult.Errors[0], Is.EqualTo("Value cannot be null or empty."));
        });
    }

    public class DummyEntity : IValidatable<DummyEntity>
    {
        public string? Value { get; set; }

        public IValidator<DummyEntity> GetValidator()
            => new DummyValidator();
    }

    public class DummyValidator : IValidator<DummyEntity>
    {
        public ValidationResult Validate(DummyEntity entity)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(entity.Value))
            {
                errors.Add("Value cannot be null or empty.");
            }

            return ValidationResult.FromErrors(errors);
        }
    }
}
