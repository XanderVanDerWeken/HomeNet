using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Queries;

public static class AllCategories
{
    public sealed record Query : IQuery;

    public sealed class QueryHandler : IQueryHandler<Query, IReadOnlyList<Category>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public QueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Result<IReadOnlyList<Category>>> HandleAsync(
            Query query, CancellationToken cancellationToken = default)
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync(
                cancellationToken);
            
            return Result<IReadOnlyList<Category>>.Success(categories);
        }
    }
}
