using HomeNet.Core.Common;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Abstractions;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetAllCategoriesAsync(
        CancellationToken cancellationToken = default);

    Task<Category?> GetCategoryByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Category?> GetCategoryByNameAsync(
        string name, 
        CancellationToken cancellationToken = default);

    Task<Result> AddCategoryAsync(
        Category category,
        CancellationToken cancellationToken = default);
}
