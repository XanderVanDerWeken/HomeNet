using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Finances.Abstractions;

namespace HomeNet.Core.Modules.Finances.Queries;

public static class MonthlySummary
{
    public sealed record Query : IQuery, IValidatable<Query>
    {
        public required int Year { get; init; }

        public required int Month { get; init; }

        public ValidationResult Validate()
            => new QueryValidator().Validate(this);
    }

    public sealed class QueryHandler : IQueryHandler<Query, Models.MonthlySummary>
    {
        private readonly ITransactionRepository _transactionRepository;

        public QueryHandler(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<Result<Models.MonthlySummary>> HandleAsync(
            Query query, CancellationToken cancellationToken = default)
        {
            var validationResult = query.Validate();

            if (!validationResult.IsValid)
            {
                return validationResult.ToFailure<Models.MonthlySummary>();
            }

            var summary = await _transactionRepository.GetMonthlySummaryAsync(
                query.Year, query.Month, cancellationToken);
            return Result<Models.MonthlySummary>.Success(summary);
        }
    }

    private sealed class QueryValidator : BaseValidator<Query>
    {
        protected override void ValidateInternal(Query entity)
        {
            IsValidFinanceYear(entity.Year, "Year must be between 2000 and the current year.");

            IsValidFinanceMonth(entity.Month, "Month must be between 1 and 12.");
        }
    }
}
