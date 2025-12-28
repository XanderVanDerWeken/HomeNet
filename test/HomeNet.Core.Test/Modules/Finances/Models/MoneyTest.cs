using HomeNet.Core.Modules.Finances.Models;
using NUnit.Framework;

namespace HomeNet.Core.Test.Modules.Finances.Models;

public class MoneyTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Should_CreateMoneyInstance()
    {
        // Arrange
        var amount = 100;
        var zeroAmount = 0;
        var invalidAmount = -50;

        // Act
        var money = new Money(amount);
        var zeroMoney = new Money(zeroAmount);

        Money invalidMoney;
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            invalidMoney = new Money(invalidAmount);
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(money.Amount, Is.EqualTo(amount));

            Assert.That(zeroMoney.Amount, Is.EqualTo(zeroAmount));
            Assert.That(zeroMoney, Is.EqualTo(Money.Zero));
        });
    }

    [TestCase(10, 10)]
    [TestCase(200, 100)]
    [TestCase(45.05, 10)]
    public void Should_AddMoney(decimal amount1, decimal amount2)
    {
        // Arrange
        var money1 = new Money(amount1);
        var money2 = new Money(amount2);

        // Act
        var result = money1 + money2;

        // Assert
        var expectedMoney = new Money(amount1 + amount2);
        Assert.That(result, Is.EqualTo(expectedMoney));
    }

    [TestCase(10, 10)]
    [TestCase(200, 100)]
    [TestCase(45.05, 10)]
    public void Should_SubtractMoney(decimal amount1, decimal amount2)
    {
        // Arrange
        var money1 = new Money(amount1);
        var money2 = new Money(amount2);

        // Act
        var result = money1 - money2;

        // Assert
        var expectedMoney = new Money(amount1 - amount2);
        Assert.That(result, Is.EqualTo(expectedMoney));
    }

    [TestCase(20, 40)]
    [TestCase(50.99, 100.01)]
    public void Should_NotSubtractMoney_SecondIsBigger(decimal amount1, decimal amount2)
    {
        // Arrange
        var money1 = new Money(amount1);
        var money2 = new Money(amount2);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var result = money1 - money2;
        });
    }
}
