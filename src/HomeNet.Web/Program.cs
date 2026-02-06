using HomeNet.Web.Components;
using HomeNet.Web.Configurations;
using HomeNet.Web.Database;
using HomeNet.Web.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
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
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.AccessDeniedPath = "/access-denied";
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

        using (var scope = app.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<SqliteCacheInitializer>();
            initializer.Initialize();
        }

        app.Run();

        Log.Logger.Information("After Run");
    }
}
