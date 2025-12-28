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
        Date = new DateOnly(2024, 1, 15),
        Store = "Grocery Store",
        Category = _category1,
    };

    private static readonly Expense _expense2 = new Expense
    {
        Id = 2,
        Amount = new Money(30),
        Date = new DateOnly(2024, 1, 20),
        Store = "Supermarket",
        Category = _category1,
    };

    private static readonly Expense _expense3 = new Expense
    {
        Id = 3,
        Amount = new Money(70),
        Date = new DateOnly(2024, 2, 5),
        Store = "Fuel Station",
        Category = _category2,
    };

    private static readonly Income _income1 = new Income
    {
        Id = 4,
        Amount = new Money(2000),
        Date = new DateOnly(2024, 1, 14),
        Source = "Company Inc.",
        Category = _category3,
    };

    private static readonly Income _income2 = new Income
    {
        Id = 4,
        Amount = new Money(2000),
        Date = new DateOnly(2024, 2, 14),
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
    public async Task Should_BuildMonthlyTimelineAsync()
    {
        // Arrange
        CancellationToken ct = default;

        _transactionRepositoryMock
            .Setup(r => r.GetAllExpensesAsync(2024, 1, ct))
            .ReturnsAsync([_expense1, _expense2]);
        
        _transactionRepositoryMock
            .Setup(r => r.GetAllIncomesAsync(2024, 1, ct))
            .ReturnsAsync([_income1]);
        
        _transactionRepositoryMock
            .Setup(r => r.GetAllExpensesAsync(2024, 2, ct))
            .ReturnsAsync([_expense3]);
        
        _transactionRepositoryMock
            .Setup(r => r.GetAllIncomesAsync(2024, 2, ct))
            .ReturnsAsync([_income2]);

        _transactionRepositoryMock
            .Setup(r => r.GetAllExpensesAsync(2024, 3, ct))
            .ReturnsAsync([]);
        
        _transactionRepositoryMock
            .Setup(r => r.GetAllIncomesAsync(2024, 3, ct))
            .ReturnsAsync([]);

        // Act
        var resultMonth1 = await _timelineBuilder
            .BuildMonthlyTimelineAsync(2024, 1, ct);
        var resultMonth2 = await _timelineBuilder
            .BuildMonthlyTimelineAsync(2024, 2, ct);
        var resultMonth3 = await _timelineBuilder
            .BuildMonthlyTimelineAsync(2024, 3, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultMonth1, Is.Not.Null);
            Assert.That(resultMonth1.Year, Is.EqualTo(2024));
            Assert.That(resultMonth1.Month, Is.EqualTo(1));
            Assert.That(resultMonth1.ExpenseAmount, Is.EqualTo(new Money(80)));
            Assert.That(resultMonth1.IncomeAmount, Is.EqualTo(new Money(2000)));
            Assert.That(resultMonth1.NetTotal, Is.EqualTo(new Money(1920)));

            Assert.That(resultMonth2, Is.Not.Null);
            Assert.That(resultMonth2.Year, Is.EqualTo(2024));
            Assert.That(resultMonth2.Month, Is.EqualTo(2));
            Assert.That(resultMonth2.ExpenseAmount, Is.EqualTo(new Money(70)));
            Assert.That(resultMonth2.IncomeAmount, Is.EqualTo(new Money(2000)));
            Assert.That(resultMonth2.NetTotal, Is.EqualTo(new Money(1930)));

            Assert.That(resultMonth3, Is.Not.Null);
            Assert.That(resultMonth3.Year, Is.EqualTo(2024));
            Assert.That(resultMonth3.Month, Is.EqualTo(3));
            Assert.That(resultMonth3.ExpenseAmount, Is.EqualTo(new Money(0)));
            Assert.That(resultMonth3.IncomeAmount, Is.EqualTo(new Money(0)));
            Assert.That(resultMonth3.NetTotal, Is.EqualTo(new Money(0)));
        });

        _transactionRepositoryMock.Verify(
            r => r.GetAllExpensesAsync(2024, 1, ct),
            Times.Once);
        _transactionRepositoryMock.Verify(
            r => r.GetAllExpensesAsync(2024, 2, ct), Times.Once);
        _transactionRepositoryMock.Verify(
            r => r.GetAllExpensesAsync(2024, 3, ct), Times.Once);

        _transactionRepositoryMock.Verify(
            r => r.GetAllIncomesAsync(2024, 1, ct), Times.Once);
        _transactionRepositoryMock.Verify(
            r => r.GetAllIncomesAsync(2024, 2, ct), Times.Once);
        _transactionRepositoryMock.Verify(
            r => r.GetAllIncomesAsync(2024, 3, ct), Times.Once);
    }

    // TODO: Add more Unit Tests
}
