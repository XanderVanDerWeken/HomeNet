using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Commands;

public sealed class AddCategoryCommandHandler : ICommandHandler<AddCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public AddCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public Task<Result> HandleAsync(
        AddCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = command.Validate();

        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.ErrorMessage!);
        }

        var newCategory = new Category
        {
            Name = command.Name,
        };

        return _categoryRepository.AddCategoryAsync(
            newCategory, 
            cancellationToken);
    }
}
