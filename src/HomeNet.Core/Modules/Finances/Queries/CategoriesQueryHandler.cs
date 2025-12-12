using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Queries;

public sealed class CategoriesQueryHandler : IQueryHandler<CategoriesQuery, IReadOnlyList<Category>>
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<IReadOnlyList<Category>>> HandleAsync(
        CategoriesQuery query, 
        CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllCategoriesAsync(cancellationToken);
        return Result<IReadOnlyList<Category>>.Success(categories);
    }
}
