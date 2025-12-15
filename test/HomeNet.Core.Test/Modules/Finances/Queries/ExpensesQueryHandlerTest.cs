using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Core.Modules.Finances.Queries;
using Moq;

namespace HomeNet.Core.Test.Modules.Finances.Queries;

public class ExpensesQueryHandlerTest
{
    private ExpensesQueryHandler _handler;

    private Mock<ITransactionRepository> _transactionRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _transactionRepositoryMock = new Mock<ITransactionRepository>();

        _handler = new ExpensesQueryHandler(
            _transactionRepositoryMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsList()
    {
        // Arrange
        var ct = new CancellationToken();

        var year = 2025;
        var month = 1;

        var query = new ExpensesQuery
        {
            Year = year,
            Month = month,
        };

        var expense1 = new Expense
        {
            Id = 1,
            Amount = new Money(50),
            Date = new DateOnly(year, month, 1),
            Category = new Category
            {
                Id = 1,
                Name = "Groceries",
            },
            Store = "Grocery Store",
        };

        var expense2 = new Expense
        {
            Id = 2,
            Amount = new Money(90),
            Date = new DateOnly(year, month, 2),
            Category = new Category
            {
                Id = 2,
                Name = "Utilities",
            },
            Store = "Utility Store",
        };

        _transactionRepositoryMock
            .Setup(x => x.GetAllExpensesAsync(year, month, ct))
            .ReturnsAsync([expense1, expense2]);

        // Act
        var result = await _handler.HandleAsync(query, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Not.Null);

            Assert.That(result.Value!, Has.Count.EqualTo(2));
            Assert.That(result.Value, Does.Contain(expense1));
            Assert.That(result.Value, Does.Contain(expense2));
        });

        _transactionRepositoryMock.Verify(
            r => r.GetAllExpensesAsync(year, month, ct),
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

        var queryYearTooHigh = new ExpensesQuery
        {
            Year = yearTooHigh,
            Month = month,
        };
        var queryYearTooLow = new ExpensesQuery
        {
            Year = yearTooLow,
            Month = month,
        };
        var queryMonthTooHigh = new ExpensesQuery
        {
            Year = year,
            Month = monthTooHigh,
        };
        var queryMonthTooLow = new ExpensesQuery
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
