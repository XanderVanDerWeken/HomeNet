using Castle.Components.DictionaryAdapter;
using HomeNet.Core.Common.Validation;
using NUnit.Framework;

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
    [TestCase(0f, true)]
    [TestCase(-500, false)]
    public void Should_Validate_FloatProperty(
        float someNumber,
        bool expectedIsValid)
    {
        // Arrange
        var entity = new DummyEntity
        {
            SomeNumber = someNumber,
        };

        // Act
        var result = entity.Validate();

        // Assert
        Assert.That(result.IsValid, Is.EqualTo(expectedIsValid));
    }

    public class DummyEntity : IValidatable<DummyEntity>
    {
        public string StringValue { get; set; } = "NotEmpty";

        public float SomeNumber { get; set; }

        public ValidationResult Validate()
            => new DummValidator().Validate(this);
    }

    public class DummValidator : BaseValidator<DummyEntity>
    {
        protected override void ValidateInternal(DummyEntity entity)
        {
            IsNotEmpty(entity.StringValue, "StringValue cannot be null or empty.");
        
            IsGreaterThanZero(entity.SomeNumber, "SomeNumber must be greater than zero.");
        }
    }
}
