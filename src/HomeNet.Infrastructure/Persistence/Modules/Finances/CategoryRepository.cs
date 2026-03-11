using HomeNet.Core.Common;
using HomeNet.Core.Common.Errors;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Extensions;
using Microsoft.Extensions.Logging;
using SqlKata;

namespace HomeNet.Infrastructure.Persistence.Modules.Finances;

public sealed class CategoryRepository : SqlKataRepository, ICategoryRepository
{
    private static readonly string TableName = "finances.categories";

    private readonly ILogger _logger;

    public CategoryRepository(
        ILogger<CategoryRepository> logger,
        PostgresQueryFactory db)
        : base(db)
    {
        _logger = logger;
    }

    public async Task<Category?> GetCategoryByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var query = new Query(TableName)
            .Where("name", name);
        
        var entities = await FirstOrDefaultAsync<CategoryEntity>(query, cancellationToken);

        return entities?.ToCategory();
    }

    public async Task<IReadOnlyList<Category>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var query = new Query(TableName);

        var entities = await GetMultipleAsync<CategoryEntity>(query, cancellationToken);

        return entities
            .Select(e => e.ToCategory())
            .ToList();
    }

    public async Task<Result> AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Inserting new category");
            var query = new Query(TableName).AsInsert(new
            {
                name = category.Name,
            });

            var categoryId = await InsertAndReturnIdAsync(query);
            category.Id = categoryId;

            _logger.LogInformation("Category inserted successfully with ID: {CategoryId}", categoryId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding category with name: {Name}", category.Name);
            return new DatabaseError(TableName, ex).ToFailure();
        }
    }
}
