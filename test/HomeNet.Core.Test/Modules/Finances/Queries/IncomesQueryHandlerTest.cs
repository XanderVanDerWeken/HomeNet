using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Core.Modules.Finances.Queries;
using Moq;

namespace HomeNet.Core.Test.Modules.Finances.Queries;

public class IncomesQueryHandlerTest
{
    private IncomesQueryHandler _handler;

    private Mock<ITransactionRepository> _transactionRepositoryMock; 

    [SetUp]
    public void Setup()
    {
        _transactionRepositoryMock = new Mock<ITransactionRepository>();

        _handler = new IncomesQueryHandler(
            _transactionRepositoryMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsList()
    {
        // Arrange
        var ct = new CancellationToken();

        var year = 2025;
        var month = 1;

        var query = new IncomesQuery
        {
            Year = year,
            Month = month,
        };

        var income1 = new Income
        {
            Id = 1,
            Amount = new Money(2000),
            Date = new DateOnly(year, month, 1),
            Category = new Category
            {
                Id = 1,
                Name = "Salary",
            },
            Source = "Job",
        };

        var income2 = new Income
        {
            Id = 2,
            Amount = new Money(500),
            Date = new DateOnly(year, month, 1),
            Category = new Category
            {
                Id = 1,
                Name = "Salary",
            },
            Source = "Job",
        };

        _transactionRepositoryMock
            .Setup(x => x.GetAllIncomesAsync(year, month, ct))
            .ReturnsAsync([income1, income2]);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Not.Null);

            Assert.That(result.Value!, Has.Count.EqualTo(2));
            Assert.That(result.Value, Does.Contain(income1));
            Assert.That(result.Value, Does.Contain(income2));
        });

        _transactionRepositoryMock.Verify(
            r => r.GetAllIncomesAsync(year, month, ct),
            Times.Once);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_InvalidQuery()
    {
        // Arrange
        var ct = new CancellationToken();

        var yearTooHigh = 3001;
        var yearTooLow = 0;
        var monthTooHigh = 13;
        var monthTooLow = 0;

        var year = 2025;
        var month = 1;

        var queryYearTooHigh = new IncomesQuery
        {
            Year = yearTooHigh,
            Month = month,
        };
        var queryYearTooLow = new IncomesQuery
        {
            Year = yearTooLow,
            Month = month,
        };
        var queryMonthTooHigh = new IncomesQuery
        {
            Year = year,
            Month = monthTooHigh,
        };
        var queryMonthTooLow = new IncomesQuery
        {
            Year = year,
            Month = monthTooLow,
        };

        // Act
        var resultYearTooHigh = await _handler.HandleAsync(queryYearTooHigh, ct);
        var resultYearTooLow = await _handler.HandleAsync(queryYearTooLow, ct);
        var resultMonthTooHigh = await _handler.HandleAsync(queryMonthTooHigh, ct);
        var resultMonthTooLow = await _handler.HandleAsync(queryMonthTooLow, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultYearTooHigh.IsSuccess, Is.False);
            Assert.That(resultYearTooHigh.Error, Is.Not.Null);
            
            Assert.That(resultYearTooLow.IsSuccess, Is.False);
            Assert.That(resultYearTooLow.Error, Is.Not.Null);

            Assert.That(resultMonthTooHigh.IsSuccess, Is.False);
            Assert.That(resultMonthTooHigh.Error, Is.Not.Null);

            Assert.That(resultMonthTooLow.IsSuccess, Is.False);
            Assert.That(resultMonthTooLow.Error, Is.Not.Null);
        });
    }
}
