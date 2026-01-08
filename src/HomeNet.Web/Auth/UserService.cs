using System.Security.Claims;
using HomeNet.Core.Modules.Auth.Extensions;
using HomeNet.Core.Modules.Auth.Models;
using Microsoft.AspNetCore.Authentication;

namespace HomeNet.Web.Auth;

public sealed class UserService
{
    public async Task SignInAsync(HttpContext context, User user)
    {
        List<Claim> claims = [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToRoleString()),
        ];

        var claimsIdentity = new ClaimsIdentity(claims, "HomeNetCookie");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        await context.SignInAsync("HomeNetCookie", claimsPrincipal);
    }
}
