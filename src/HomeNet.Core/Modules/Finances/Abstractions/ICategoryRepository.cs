using HomeNet.Core.Common;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Abstractions;

public interface ICategoryRepository
{
    public Task<IReadOnlyList<Category>> GetAllCategoriesAsync(
        CancellationToken cancellationToken = default);

    public Task<Category?> GetCategoryByNameAsync(
        string name,
        CancellationToken cancellationToken = default);

    public Task<Result> AddCategoryAsync(
        Category category, 
        CancellationToken cancellationToken = default);
}
