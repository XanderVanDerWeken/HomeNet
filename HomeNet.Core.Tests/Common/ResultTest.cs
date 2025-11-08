namespace HomeNet.Core.Tests.Common;

using HomeNet.Core.Common;

public class ResultTest
{
    [Test]
    public void Test_Failure_WithErrorNone()
    {
        Assert.Throws<ArgumentException>(() => Result.Failure(Error.None));
        Assert.Throws<ArgumentException>(() => Result<object>.Failure(Error.None));
    }
}
