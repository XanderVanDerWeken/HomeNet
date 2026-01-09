using HomeNet.Web.Auth;

namespace HomeNet.Web.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddHomeNetAuth(this IServiceCollection services)
    {
        services.AddSingleton<IAuthService, AuthService>();

        services.AddAuthentication("HomeNetCookie")
            .AddCookie("HomeNetCookie", options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.AccessDeniedPath = "/forbidden";

                options.Cookie.Name = "HomeNet.Auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
            });
        
        services.AddAuthorization();


        return services;
    }
}
