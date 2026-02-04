using HomeNet.Infrastructure.Security;

namespace HomeNet.Infrastructure.Test.Security;

public class PasswordServiceTest
{
    private PasswordService passwordService;

    [SetUp]
    public void Setup()
    {
        passwordService = new PasswordService();
    }

    [TestCase("Password123!")]
    [TestCase("Test1")]
    public void Should_HashAndVerifyPassword(string passwordString)
    {
        // Act
        var hashedPassword = passwordService.HashPassword(passwordString);
        var isVerified = passwordService.VerifyPassword(passwordString, hashedPassword);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(hashedPassword, Is.Not.EqualTo(passwordString));
            Assert.That(isVerified, Is.True);
        });
    }
}
