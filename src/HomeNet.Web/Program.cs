using System.Security.Claims;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Auth.Queries;
using HomeNet.Web.Components;
using HomeNet.Web.Configurations;
using HomeNet.Web.Database;
using HomeNet.Web.Extensions;
using HomeNet.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using MudBlazor.Services;
using Serilog;

namespace HomeNet.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(Log.Logger);

        builder.Services.Configure<CacheInitializerConfiguration>(
            builder.Configuration.GetSection(nameof(CacheInitializerConfiguration)));

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        builder.Services.AddMudServices();
        
        builder.Services.AddCommonServices(builder.Configuration);
        
        builder.Services.AddCardsModule();

        builder.Services.AddScoped<SqliteCacheInitializer>(sp =>
        {
            return new SqliteCacheInitializer(
                sp.GetRequiredService<ILogger<SqliteCacheInitializer>>(),
                builder.Configuration.GetConnectionString("Cache")!,
                sp.GetRequiredService<IOptions<CacheInitializerConfiguration>>());
        });

        // Authentication and Authorization
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.AccessDeniedPath = "/forbidden";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
            });
        
        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
        app.UseHttpsRedirection();

        app.UseAntiforgery();

        // Authentication and Authorization Middlewares
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

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

            return Results.Unauthorized();
        });

        app.MapPost("/api/auth/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Ok();
        });

        using (var scope = app.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<SqliteCacheInitializer>();
            initializer.Initialize();
        }

        app.Run();

        Log.Logger.Information("After Run");
    }
}
