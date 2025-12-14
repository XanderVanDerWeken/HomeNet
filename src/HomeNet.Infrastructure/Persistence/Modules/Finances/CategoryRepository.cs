using HomeNet.Core.Common;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using SqlKata;

namespace HomeNet.Infrastructure.Persistence.Modules.Finances;

public sealed class CategoryRepository : SqlKataRepository, ICategoryRepository
{
    public CategoryRepository(PostgresQueryFactory db)
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

    public async Task<Category?> GetCategoryByNameAsync(
        string name, 
        CancellationToken cancellationToken = default)
    {
        var query = new Query("categories")
            .Where("name", name);
        
        var row = await FirstOrDefaultAsync<Category>(
            query,
            cancellationToken);
        
        return row;
    }

    public async Task<Result> AddCategoryAsync(
        Category category, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new Query("categories")
                .AsInsert(new
                {
                    name = category.Name,
                });

            var rows = await ExecuteAsync(query, cancellationToken);

            return rows > 0
                ? Result.Success()
                : Result.Failure("Failed to insert category into database.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while adding the category: {ex.Message}");
        }
    }
}
