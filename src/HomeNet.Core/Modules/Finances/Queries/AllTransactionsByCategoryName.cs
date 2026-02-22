using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Errors;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Queries;

public static class AllTransactionsByCategoryName
{
    public sealed record Query : IQuery, IValidatable<Query>
    {
        public required string CategoryName { get; init; }

        public ValidationResult Validate()
            => new QueryValidator().Validate(this);
    }

    public sealed class QueryHandler : IQueryHandler<Query, IReadOnlyList<Transaction>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICategoryRepository _categoryRepository;

        public QueryHandler(
            ITransactionRepository transactionRepository, 
            ICategoryRepository categoryRepository)
        {
            _transactionRepository = transactionRepository;
            _categoryRepository = categoryRepository;
        }
        
        public async Task<Result<IReadOnlyList<Transaction>>> HandleAsync(
            Query query, CancellationToken cancellationToken = default)
        {
            var validationResult = query.Validate();

            if (!validationResult.IsValid)
            {
                return validationResult.ToFailure<IReadOnlyList<Transaction>>();
            }

            var foundCategory = await _categoryRepository.GetCategoryByNameAsync(
                query.CategoryName, cancellationToken);
            
            if (foundCategory is null)
            {
                var error = new NotFoundError("Category", query.CategoryName);
                return Result<IReadOnlyList<Transaction>>.Failure(error);
            }

            var transactions =  await _transactionRepository.GetTransactionsWithCategoryIdAsync(
                foundCategory.Id, cancellationToken);
            return Result<IReadOnlyList<Transaction>>.Success(transactions);
        }
    }

    private sealed class QueryValidator : BaseValidator<Query>
    {
        protected override void ValidateInternal(Query entity)
        {
            IsNotEmpty(entity.CategoryName, "CategoryName cannot be empty.");
        }
    }
}
