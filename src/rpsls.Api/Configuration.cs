using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using rpsls.Infrastructure.Database;

namespace rpsls.Api;

public static class Configuration
{
    public static void RegisterApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddDatabase(configuration);
    }
    
    private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<ApplicationDbContext>(options => options
            .UseNpgsql(connectionString, npqsqlOptions => npqsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "public"))
            .UseSnakeCaseNamingConvention()
        );

        // This allows you to inject the interface elsewhere instead of the concrete DbContext
        //services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
    }

}