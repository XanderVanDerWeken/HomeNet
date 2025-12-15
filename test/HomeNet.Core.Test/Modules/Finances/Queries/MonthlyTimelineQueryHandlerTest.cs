using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Core.Modules.Finances.Queries;
using Moq;

namespace HomeNet.Core.Test.Modules.Finances.Queries;

public class MonthlyTimelineQueryHandlerTest
{
    private MonthlyTimelineQueryHandler _handler;

    private Mock<ITimelineBuilder> _timelineBuilderMock;
    
    [SetUp]
    public void Setup()
    {
        _timelineBuilderMock = new Mock<ITimelineBuilder>();

        _handler = new MonthlyTimelineQueryHandler(
            _timelineBuilderMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsMonthlyTimeline()
    {
        // Arrange
        var ct = new CancellationToken();

        var year = 2025;
        var month = 1;

        var query = new MonthlyTimelineQuery
        {
            Year = year,
            Month = month,
        };

        var expenseAmount = new Money(75m);
        var incomeAmount = new Money(100m);
        var netTotal = new Money(25m);

        var monthlyTimeline = new MonthlyTimeline
        {
            Year = year,
            Month = month,
            ExpenseAmount = expenseAmount,
            IncomeAmount = incomeAmount,
            NetTotal = netTotal,
        };

        _timelineBuilderMock
            .Setup(x => x.GetOrCreateMonthlyTimelineAsync(year, month, ct))
            .ReturnsAsync(monthlyTimeline);

        // Act
        var result = await _handler.HandleAsync(query, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Not.Null);

            Assert.That(result.Value!.Year, Is.EqualTo(year));
            Assert.That(result.Value.Month, Is.EqualTo(month));
            Assert.That(result.Value.ExpenseAmount, Is.EqualTo(expenseAmount));
            Assert.That(result.Value.IncomeAmount, Is.EqualTo(incomeAmount));
            Assert.That(result.Value.NetTotal, Is.EqualTo(netTotal));
        });

        _timelineBuilderMock.Verify(
            tb => tb.GetOrCreateMonthlyTimelineAsync(year, month, ct),
            Times.Once);
    }

    [Test]
    public async Task Should_HandleAsync_InvalidQuery_ReturnsFailure()
    {
        // Arrange
        var ct = new CancellationToken();

        var yearTooHigh = 3001;
        var yearTooLow = 0;
        var monthTooHigh = 13;
        var monthTooLow = 0;

        var year = 2025;
        var month = 1;

        var queryYearTooHigh = new MonthlyTimelineQuery
        {
            Year = yearTooHigh,
            Month = month,
        };
        var queryYearTooLow = new MonthlyTimelineQuery
        {
            Year = yearTooLow,
            Month = month,
        };
        var queryMonthTooHigh = new MonthlyTimelineQuery
        {
            Year = year,
            Month = monthTooHigh,
        };
        var queryMonthTooLow = new MonthlyTimelineQuery
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
