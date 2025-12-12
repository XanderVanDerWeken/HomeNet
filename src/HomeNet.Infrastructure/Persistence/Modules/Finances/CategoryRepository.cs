using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;
using SqlKata;
using SqlKata.Execution;

namespace HomeNet.Infrastructure.Persistence.Modules.Finances;

public sealed class CategoryRepository : SqlKataRepository, ICategoryRepository
{
    public CategoryRepository(QueryFactory db)
        : base(db)
    {
    }

    public async Task<IReadOnlyList<Category>> GetAllCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        var query = new Query("categories").OrderBy("id");
    
        return await GetListAsync<Category>(
            query,
            cancellationToken);
    }
}
