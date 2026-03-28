using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Commands;

public static class CreateCategory
{
    public sealed record Command : ICommand, IValidatable<Command>
    {
        public required string Name { get; init; }

        public ValidationResult Validate()
            => new CommandValidator().Validate(this);
    }

    public sealed class CommandHandler : ICommandHandler<Command, Category>
    {
        private readonly ICategoryRepository _categoryRepository;

        public CommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public Task<Result<Category>> HandleAsync(
            Command command, CancellationToken cancellationToken = default)
        {
            var validationResult = command.Validate();

            if (!validationResult.IsValid)
            {
                return validationResult.ToFailure<Category>();
            }

            var newCategory = new Category
            {
                Name = command.Name,
            };

            return _categoryRepository.AddCategoryAsync(newCategory, cancellationToken);
        }
    }

    private sealed class CommandValidator : BaseValidator<Command>
    {
        protected override void ValidateInternal(Command entity)
        {
            IsNotEmpty(entity.Name, "Name is required.");
        }
    }
}
