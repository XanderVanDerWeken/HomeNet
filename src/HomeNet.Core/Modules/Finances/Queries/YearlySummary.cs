using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Finances.Abstractions;

namespace HomeNet.Core.Modules.Finances.Queries;

public static class YearlySummary
{
    public sealed record Query : IQuery, IValidatable<Query>
    {
        public required int Year { get; init; }

        public ValidationResult Validate()
            => new QueryValidator().Validate(this);
    }

    public sealed class QueryHandler : IQueryHandler<Query, Models.YearlySummary>
    {
        private readonly ITransactionRepository _transactionRepository;

        public QueryHandler(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<Result<Models.YearlySummary>> HandleAsync(
            Query query, CancellationToken cancellationToken = default)
        {
            var validationResult = query.Validate();

            if (!validationResult.IsValid)
            {
                return validationResult.ToFailure<Models.YearlySummary>();
            }

            var summary = await _transactionRepository.GetYearlySummaryAsync(
                query.Year, cancellationToken);
            return Result<Models.YearlySummary>.Success(summary);
        }
    }

    private sealed class QueryValidator : BaseValidator<Query>
    {
        protected override void ValidateInternal(Query entity)
        {
            IsValidFinanceYear(entity.Year, "Year must be between 2000 and the current year.");
        }
    }
}
