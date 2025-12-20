using HomeNet.Core.Common;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Extensions;
using SqlKata;

namespace HomeNet.Infrastructure.Persistence.Modules.Finances;

public sealed class CategoryRepository : SqlKataRepository, ICategoryRepository
{
    private static readonly string TableName = "finances.categories";

    public CategoryRepository(PostgresQueryFactory db)
        : base(db)
    {
    }

    public async Task<IReadOnlyList<Category>> GetAllCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        var query = new Query(TableName);
    
        var entities = await GetMultipleAsync<CategoryEntity>(
            query, 
            cancellationToken);
        
        return entities
            .Select(e => e.ToCategory())
            .ToList();
    }

    public async Task<Category?> GetCategoryByNameAsync(
        string name, 
        CancellationToken cancellationToken = default)
    {
        var query = new Query(TableName)
            .Where("name", name);
        
        var row = await FirstOrDefaultAsync<CategoryEntity>(
            query,
            cancellationToken);
        
        return row?.ToCategory();
    }

    public async Task<Result> AddCategoryAsync(
        Category category, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new Query(TableName)
                .AsInsert(new
                {
                    name = category.Name,
                });

            var newCategoryId = await InsertAndReturnIdAsync(query);
            category.Id = newCategoryId;

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while adding the category: {ex.Message}");
        }
    }
}
