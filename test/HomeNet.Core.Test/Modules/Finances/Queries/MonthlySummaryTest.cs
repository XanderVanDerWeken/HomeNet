using HomeNet.Core.Common.Errors;
using HomeNet.Core.Modules.Finances.Abstractions;
using ModelMonhtlySummary = HomeNet.Core.Modules.Finances.Models.MonthlySummary;
using Moq;
using HomeNet.Core.Modules.Finances.Models;
using MonthlySummary = HomeNet.Core.Modules.Finances.Queries.MonthlySummary;

namespace HomeNet.Core.Test.Modules.Finances.Queries;

public class MonthlySummaryTest
{
    private MonthlySummary.QueryHandler _handler;

    private Mock<ITransactionRepository> _transactionRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _transactionRepositoryMock = new Mock<ITransactionRepository>();

        _handler = new MonthlySummary.QueryHandler(
            _transactionRepositoryMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsSuccess()
    {
        // Arrange
        var ct = new CancellationToken();

        var query = new MonthlySummary.Query
        {
            Year = DateTime.Now.Year,
            Month = DateTime.Now.Month,
        };

        var monthlySummary = new ModelMonhtlySummary
        {
            Year = query.Year,
            Month = query.Month,
            TotalIncome = new Money(1000),
            TotalExpenses = new Money(500),
        };

        _transactionRepositoryMock
            .Setup(x => x.GetMonthlySummaryAsync(query.Year, query.Month, ct))
            .ReturnsAsync(monthlySummary);

        // Act
        var result = await _handler.HandleAsync(query, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(monthlySummary));
        });

        _transactionRepositoryMock.Verify(
            x => x.GetMonthlySummaryAsync(query.Year, query.Month, ct), 
            Times.Once);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_InvalidQuery()
    {
        // Arrange
        var ct = new CancellationToken();

        var queryYearTooLow = new MonthlySummary.Query
        {
            Year = 1999,
            Month = 1,
        };
        var queryYearTooHigh = new MonthlySummary.Query
        {
            Year = DateTime.Now.Year + 1,
            Month = 1,
        };

        var queryMonthTooLow = new MonthlySummary.Query
        {
            Year = DateTime.Now.Year,
            Month = 0,
        };
        var queryMonthTooHigh = new MonthlySummary.Query
        {
            Year = DateTime.Now.Year,
            Month = 13,
        };

        // Act
        var resultYearTooLow = await _handler.HandleAsync(queryYearTooLow, ct);
        var resultYearTooHigh = await _handler.HandleAsync(queryYearTooHigh, ct);
        var resultMonthTooLow = await _handler.HandleAsync(queryMonthTooLow, ct);
        var resultMonthTooHigh = await _handler.HandleAsync(queryMonthTooHigh, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultYearTooLow.IsSuccess, Is.False);
            Assert.That(resultYearTooLow.Error, Is.Not.Null);
            Assert.That(resultYearTooLow.Error, Is.InstanceOf<ValidationError>());

            Assert.That(resultYearTooHigh.IsSuccess, Is.False);
            Assert.That(resultYearTooHigh.Error, Is.Not.Null);
            Assert.That(resultYearTooHigh.Error, Is.InstanceOf<ValidationError>());

            Assert.That(resultMonthTooLow.IsSuccess, Is.False);
            Assert.That(resultMonthTooLow.Error, Is.Not.Null);
            Assert.That(resultMonthTooLow.Error, Is.InstanceOf<ValidationError>());

            Assert.That(resultMonthTooHigh.IsSuccess, Is.False);
            Assert.That(resultMonthTooHigh.Error, Is.Not.Null);
            Assert.That(resultMonthTooHigh.Error, Is.InstanceOf<ValidationError>());
        });
    }
}
