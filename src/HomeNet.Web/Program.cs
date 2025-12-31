using HomeNet.Web.Components;
using HomeNet.Web.Configurations;
using HomeNet.Web.Database;
using HomeNet.Web.Extensions;
using Microsoft.Extensions.Options;
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
        
        builder.Services.AddCommonServices(builder.Configuration);
        
        builder.Services.AddCardsModule();

        builder.Services.AddScoped<SqliteCacheInitializer>(sp =>
        {
            return new SqliteCacheInitializer(
                sp.GetRequiredService<ILogger<SqliteCacheInitializer>>(),
                builder.Configuration.GetConnectionString("Cache")!,
                sp.GetRequiredService<IOptions<CacheInitializerConfiguration>>());
        });

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
