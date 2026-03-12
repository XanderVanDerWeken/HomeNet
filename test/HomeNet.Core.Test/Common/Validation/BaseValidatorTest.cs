using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Test.Common.Validation;

public class BaseValidatorTest
{
    
    [TestCase("", false)]
    [TestCase("   ", false)]
    [TestCase("ValidString", true)]
    public void Should_Validate_StringProperty(
        string categoryName,
        bool expectedIsValid)
    {
        // Arrange
        var entity = new DummyEntity
        {
            StringValue = categoryName,
        };

        // Act
        var result = entity.Validate();

        // Assert
        Assert.That(result.IsValid, Is.EqualTo(expectedIsValid));
    }

    [TestCase(500, true)]
    [TestCase(1, true)]
    [TestCase(0, false)]
    [TestCase(-500, false)]
    public void Should_Validate_IntProperty(
        int someNumber,
        bool expectedIsValid)
    {
        // Arrange
        var entity = new DummyEntity
        {
            SomeIntNumber = someNumber,
        };

        // Act
        var result = entity.Validate();

        // Assert
        Assert.That(result.IsValid, Is.EqualTo(expectedIsValid));
    }

    [TestCase(500, true)]
    [TestCase(1, true)]
    [TestCase(0f, false)]
    [TestCase(-500, false)]
    public void Should_Validate_FloatProperty(
        float someNumber,
        bool expectedIsValid)
    {
        // Arrange
        var entity = new DummyEntity
        {
            SomeFloatNumber = someNumber,
        };

        // Act
        var result = entity.Validate();

        // Assert
        Assert.That(result.IsValid, Is.EqualTo(expectedIsValid));
    }

    public class DummyEntity : IValidatable<DummyEntity>
    {
        public string StringValue { get; set; } = "NotEmpty";

        public float SomeFloatNumber { get; set; } = 1.0f;

        public float SomeIntNumber { get; set; } = 1.0f;

        public ValidationResult Validate()
            => new DummValidator().Validate(this);
    }

    public class DummValidator : BaseValidator<DummyEntity>
    {
        protected override void ValidateInternal(DummyEntity entity)
        {
            IsNotEmpty(entity.StringValue, "StringValue cannot be null or empty.");
        
            IsGreaterThanZero(entity.SomeFloatNumber, "SomeFloatNumber must be greater than zero.");

            IsGreaterThanZero(entity.SomeIntNumber, "SomeIntNumber must be greater than zero.");
        }
    }
}
