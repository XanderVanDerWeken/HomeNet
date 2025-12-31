using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Core.Modules.Finances.Services;
using Moq;

namespace HomeNet.Core.Test.Modules.Finances.Services;

public class TimelineBuilderTest
{
    private TimelineBuilder _timelineBuilder;
    
    private Mock<IMonthlyTimelineRepository> _monthlyTimelineRepositoryMock;
    private Mock<ITransactionRepository> _transactionRepositoryMock;

    private static readonly int _year = 2024;
    private static readonly int _month1 = 1;
    private static readonly int _month2 = 2;
    private static readonly int _month3 = 3;

    private static readonly Category _category1 = new Category
    {
        Id = 1,
        Name = "Groceries",
    };

    private static readonly Category _category2 = new Category
    {
        Id = 2,
        Name = "Fuel",
    };

    private static readonly Category _category3 = new Category
    {
        Id = 3,
        Name = "Main Job",
    };

    private static readonly Expense _expense1 = new Expense
    {
        Id = 1,
        Amount = new Money(50),
        Date = new DateOnly(_year, _month1, 15),
        Store = "Grocery Store",
        Category = _category1,
    };

    private static readonly Expense _expense2 = new Expense
    {
        Id = 2,
        Amount = new Money(30),
        Date = new DateOnly(_year, _month1, 20),
        Store = "Supermarket",
        Category = _category1,
    };

    private static readonly Expense _expense3 = new Expense
    {
        Id = 3,
        Amount = new Money(70),
        Date = new DateOnly(_year, _month2, 5),
        Store = "Fuel Station",
        Category = _category2,
    };

    private static readonly Income _income1 = new Income
    {
        Id = 4,
        Amount = new Money(2000),
        Date = new DateOnly(_year, _month1, 14),
        Source = "Company Inc.",
        Category = _category3,
    };

    private static readonly Income _income2 = new Income
    {
        Id = 4,
        Amount = new Money(2000),
        Date = new DateOnly(_year, _month2, 14),
        Source = "Company Inc.",
        Category = _category3,
    };
 
    [SetUp]
    public void Setup()
    {
        _monthlyTimelineRepositoryMock = new Mock<IMonthlyTimelineRepository>();
        _transactionRepositoryMock = new Mock<ITransactionRepository>();

        _timelineBuilder = new TimelineBuilder(
            _monthlyTimelineRepositoryMock.Object,
            _transactionRepositoryMock.Object);
    }

    [Test]
    public async Task Should_GetOrCreateMonthlyTimelineAsync_TimelineExists()
    {
        // Arrange
        MockTransactionRepositoryLogic();

        var timeline1 = new MonthlyTimeline
        {
            Year = _year,
            Month = _month1,
            ExpenseAmount = new Money(80),
            IncomeAmount = new Money(2000),
            NetTotal = new Money(1920),
        };

        var timeline2 = new MonthlyTimeline
        {
            Year = _year,
            Month = _month2,
            ExpenseAmount = new Money(70),
            IncomeAmount = new Money(2000),
            NetTotal = new Money(1930),
        };

        _monthlyTimelineRepositoryMock
            .Setup(r => r.GetMonthlyTimelineAsync(_year, _month1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(timeline1);
        
        _monthlyTimelineRepositoryMock
            .Setup(r => r.GetMonthlyTimelineAsync(_year, _month2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(timeline2);

        // Act
        var resultMonth1 = await _timelineBuilder.GetOrCreateMonthlyTimelineAsync(_year, _month1);
        var resultMonth2 = await _timelineBuilder.GetOrCreateMonthlyTimelineAsync(_year, _month2);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultMonth1, Is.Not.Null);
            Assert.That(resultMonth1.Year, Is.EqualTo(_year));
            Assert.That(resultMonth1.Month, Is.EqualTo(_month1));
            Assert.That(resultMonth1.ExpenseAmount, Is.EqualTo(timeline1.ExpenseAmount));
            Assert.That(resultMonth1.IncomeAmount, Is.EqualTo(timeline1.IncomeAmount));
            Assert.That(resultMonth1.NetTotal, Is.EqualTo(timeline1.NetTotal));

            Assert.That(resultMonth2, Is.Not.Null);
            Assert.That(resultMonth2.Year, Is.EqualTo(_year));
            Assert.That(resultMonth2.Month, Is.EqualTo(_month2));
            Assert.That(resultMonth2.ExpenseAmount, Is.EqualTo(timeline2.ExpenseAmount));
            Assert.That(resultMonth2.IncomeAmount, Is.EqualTo(timeline2.IncomeAmount));
            Assert.That(resultMonth2.NetTotal, Is.EqualTo(timeline2.NetTotal));
        });

        _monthlyTimelineRepositoryMock.Verify(
            r => r.GetMonthlyTimelineAsync(_year, _month1, It.IsAny<CancellationToken>()), 
            Times.Once);
        
        _monthlyTimelineRepositoryMock.Verify(
            r => r.GetMonthlyTimelineAsync(_year, _month2, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Test]
    public async Task Should_GetOrCreateMonthlyTimelineAsync_TimelineDoesNotExist()
    {
        // Arrange
        MockTransactionRepositoryLogic();

        _monthlyTimelineRepositoryMock
            .Setup(r => r.GetMonthlyTimelineAsync(
                _year, _month1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((MonthlyTimeline?)null);

        // Act
        var result = await _timelineBuilder
            .GetOrCreateMonthlyTimelineAsync(_year, _month1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Year, Is.EqualTo(_year));
            Assert.That(result.Month, Is.EqualTo(_month1));
            Assert.That(result.ExpenseAmount, Is.EqualTo(new Money(80)));
            Assert.That(result.IncomeAmount, Is.EqualTo(new Money(2000)));
            Assert.That(result.NetTotal, Is.EqualTo(new Money(1920)));
        });

        _monthlyTimelineRepositoryMock.Verify(
            r => r.GetMonthlyTimelineAsync(
                _year, _month1, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Test]
    public async Task Should_RecalculateMonthlyTimelineAsync()
    {
        // Arrange
        MockTransactionRepositoryLogic();

        _monthlyTimelineRepositoryMock
            .Setup(r => r.SaveMonthlyTimelineAsync(
                It.IsAny<MonthlyTimeline>(), 
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _timelineBuilder
            .RecalculateMonthlyTimelineAsync(_year, _month1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Year, Is.EqualTo(_year));
            Assert.That(result.Month, Is.EqualTo(_month1));
            Assert.That(result.ExpenseAmount, Is.EqualTo(new Money(80)));
            Assert.That(result.IncomeAmount, Is.EqualTo(new Money(2000)));
            Assert.That(result.NetTotal, Is.EqualTo(new Money(1920)));
        });

        _monthlyTimelineRepositoryMock.Verify(
            r => r.SaveMonthlyTimelineAsync(
                It.Is<MonthlyTimeline>(t => 
                    t.Year == _year && 
                    t.Month == _month1 &&
                    t.ExpenseAmount == new Money(80) &&
                    t.IncomeAmount == new Money(2000) &&
                    t.NetTotal == new Money(1920)),
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Test]
    public async Task Should_BuildMonthlyTimelineAsync()
    {
        // Arrange
        MockTransactionRepositoryLogic();
        
        // Act
        var resultMonth1 = await _timelineBuilder
            .BuildMonthlyTimelineAsync(_year, _month1);
        var resultMonth2 = await _timelineBuilder
            .BuildMonthlyTimelineAsync(_year, _month2);
        var resultMonth3 = await _timelineBuilder
            .BuildMonthlyTimelineAsync(_year, _month3);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultMonth1, Is.Not.Null);
            Assert.That(resultMonth1.Year, Is.EqualTo(_year));
            Assert.That(resultMonth1.Month, Is.EqualTo(_month1));
            Assert.That(resultMonth1.ExpenseAmount, Is.EqualTo(new Money(80)));
            Assert.That(resultMonth1.IncomeAmount, Is.EqualTo(new Money(2000)));
            Assert.That(resultMonth1.NetTotal, Is.EqualTo(new Money(1920)));

            Assert.That(resultMonth2, Is.Not.Null);
            Assert.That(resultMonth2.Year, Is.EqualTo(_year));
            Assert.That(resultMonth2.Month, Is.EqualTo(_month2));
            Assert.That(resultMonth2.ExpenseAmount, Is.EqualTo(new Money(70)));
            Assert.That(resultMonth2.IncomeAmount, Is.EqualTo(new Money(2000)));
            Assert.That(resultMonth2.NetTotal, Is.EqualTo(new Money(1930)));

            Assert.That(resultMonth3, Is.Not.Null);
            Assert.That(resultMonth3.Year, Is.EqualTo(_year));
            Assert.That(resultMonth3.Month, Is.EqualTo(_month3));
            Assert.That(resultMonth3.ExpenseAmount, Is.EqualTo(new Money(0)));
            Assert.That(resultMonth3.IncomeAmount, Is.EqualTo(new Money(0)));
            Assert.That(resultMonth3.NetTotal, Is.EqualTo(new Money(0)));
        });

        _transactionRepositoryMock.Verify(
            r => r.GetAllExpensesAsync(_year, _month1, It.IsAny<CancellationToken>()),
            Times.Once);
        _transactionRepositoryMock.Verify(
            r => r.GetAllExpensesAsync(_year, _month2, It.IsAny<CancellationToken>()), 
            Times.Once);
        _transactionRepositoryMock.Verify(
            r => r.GetAllExpensesAsync(_year, _month3, It.IsAny<CancellationToken>()), 
            Times.Once);

        _transactionRepositoryMock.Verify(
            r => r.GetAllIncomesAsync(_year, _month1, It.IsAny<CancellationToken>()), 
            Times.Once);
        _transactionRepositoryMock.Verify(
            r => r.GetAllIncomesAsync(_year, _month2, It.IsAny<CancellationToken>()), 
            Times.Once);
        _transactionRepositoryMock.Verify(
            r => r.GetAllIncomesAsync(_year, _month3, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    private void MockTransactionRepositoryLogic()
    {
        _transactionRepositoryMock
            .Setup(r => r.GetAllExpensesAsync(_year, _month1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([_expense1, _expense2]);
        
        _transactionRepositoryMock
            .Setup(r => r.GetAllIncomesAsync(_year, _month1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([_income1]);
        
        _transactionRepositoryMock
            .Setup(r => r.GetAllExpensesAsync(_year, _month2, It.IsAny<CancellationToken>()))
            .ReturnsAsync([_expense3]);
        
        _transactionRepositoryMock
            .Setup(r => r.GetAllIncomesAsync(_year, _month2, It.IsAny<CancellationToken>()))
            .ReturnsAsync([_income2]);

        _transactionRepositoryMock
            .Setup(r => r.GetAllExpensesAsync(_year, _month3, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        
        _transactionRepositoryMock
            .Setup(r => r.GetAllIncomesAsync(_year, _month3, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
    }
}
