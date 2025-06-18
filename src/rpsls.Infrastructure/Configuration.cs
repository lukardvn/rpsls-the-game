using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.Configuration;
using rpsls.Application.Interfaces;
using rpsls.Infrastructure.Repositories;
using rpsls.Infrastructure.Services;

namespace rpsls.Infrastructure;

public static class Configuration
{
    public static void RegisterInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var randomNumberApiUrl = configuration["RandomNumberService:BaseUrl"];

        if (string.IsNullOrEmpty(randomNumberApiUrl))
            throw new InvalidConfigurationException("Random number api url is missing.");
        
        services.AddHttpClient<IRandomNumberProvider, RandomNumberProvider>(client =>
        {
            client.BaseAddress = new Uri(randomNumberApiUrl);
        });
        services.AddScoped<IScoreboardRepository, ScoreboardRepository>();
        services.AddScoped<ITimeService, TimeService>();
    }
}