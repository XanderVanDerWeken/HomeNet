using System.Security.Claims;
using HomeNet.Core.Common.Events;
using HomeNet.Core.Modules.Auth.Queries;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace HomeNet.Web.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddCookieAuthentication(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.AccessDeniedPath = "/forbidden";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
            });
        
        services.AddAuthorization();

        return services;
    }

    public static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        // Login endpoint
        app.MapPost("/api/auth/login", async (HttpContext context, HttpRequest request, IEventBus bus) =>
        {
            var query = new UserWithCredentialsQuery
            {
                UserName = request.Form["username"]!,
                Password = request.Form["password"]!,
            };

            var queryResult = await bus.SendAsync<bool>(query);

            if (queryResult.IsSuccess && queryResult.Value)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, query.UserName),
                    new Claim(ClaimTypes.Role, "User"), // TODO: Query user role
                };

                var identity = new ClaimsIdentity(
                    claims, 
                    CookieAuthenticationDefaults.AuthenticationScheme);
                
                await context.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, 
                    new ClaimsPrincipal(identity));
                
                return Results.Redirect("/");
            }

            return Results.Redirect("/login?error=1");
        });

        // Logout endpoint
        app.MapPost("/api/auth/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Ok();
        });

        return app;
    }
}
