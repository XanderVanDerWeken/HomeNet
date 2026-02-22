using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Auth.Abstractions;

namespace HomeNet.Core.Modules.Auth.Queries;

public static class UserWithCredentials
{
    public sealed record Query : IQuery, IValidatable<Query>
    {
        public required string UserName { get; init; }

        public required string Password { get; init; }
        
        public ValidationResult Validate()
            => new QueryValidator().Validate(this);
    }

    public sealed class QueryHandler : IQueryHandler<Query, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;

        public QueryHandler(
            IUserRepository userRepository,
            IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
        }


        public async Task<Result<bool>> HandleAsync(
            Query query, CancellationToken cancellationToken = default)
        {
            var validationResult = query.Validate();

            if (!validationResult.IsValid)
            {
                return validationResult.ToFailure<bool>();
            }

            var userWithUsername = await _userRepository
                .GetUserByUsernameAsync(query.UserName);
            
            if (userWithUsername == null)
            {
                return Result<bool>.Success(false);
            }
            
            var isPasswordValid = _passwordService.VerifyPassword(
                query.Password, userWithUsername.PasswordHash);

            return Result<bool>.Success(isPasswordValid);
        }
    }

    private sealed class QueryValidator : BaseValidator<Query>
    {
        protected override void ValidateInternal(Query entity)
        {
            IsNotEmpty(entity.UserName, "UserName cannot be empty.");

            IsNotEmpty(entity.Password, "Password cannot be empty.");
        }
    }
}
