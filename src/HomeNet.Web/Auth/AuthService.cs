using System.Security.Claims;
using HomeNet.Core.Common.Events;
using HomeNet.Core.Modules.Auth.Extensions;
using HomeNet.Core.Modules.Auth.Models;
using HomeNet.Core.Modules.Auth.Queries;
using Microsoft.AspNetCore.Authentication;

namespace HomeNet.Web.Auth;

public sealed class AuthService : IAuthService
{
    private static readonly string authenticationScheme = "HomeNetCookie";

    private readonly IEventBus _eventBus;

    public AuthService(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task<bool> LoginAsync(HttpContext context, string username, string password)
    {
        var hashPassword = PasswordHasher.HashPassword(password);

        var userResult = await _eventBus.SendAsync<User?>(new UserWithUsernameQuery
        {
            Username = username,
        });

        if (!userResult.IsSuccess || userResult.Value == null)
            return false;

        var credentialsMatch = PasswordHasher.VerifyPassword(
            hashPassword, userResult.Value.PasswordHash);
        
        if (!credentialsMatch)
            return false;
        
        await SignInAsync(context, userResult.Value);
        return true;
    }

    public async Task LogoutAsync(HttpContext context)
    {
        await context.SignOutAsync(authenticationScheme);
    }

    private async Task SignInAsync(HttpContext context, User user)
    {
        List<Claim> claims = [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToRoleString()),
        ];

        var claimsIdentity = new ClaimsIdentity(claims, authenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        await context.SignInAsync(
            authenticationScheme, 
            claimsPrincipal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                IssuedUtc = DateTimeOffset.UtcNow,
            });
    }
}
