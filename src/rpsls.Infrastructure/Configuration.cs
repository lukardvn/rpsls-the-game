using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.Configuration;
using rpsls.Application.Interfaces;
using rpsls.Infrastructure.Repositories;
using rpsls.Infrastructure.Services;
using Polly;
using Polly.Extensions.Http;

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
        })
        .AddPolicyHandler((provider, request) =>
        {
            var logger = provider.GetRequiredService<ILogger<RandomNumberProvider>>();
            return GetRetryPolicy(logger);
        });
        
        services.AddScoped<IScoreboardRepository, ScoreboardRepository>();
        services.AddScoped<ITimeService, TimeService>();
    }
    
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (outcome, timespan, attempt, _) =>
                {
                    logger.LogWarning("Retry {Attempt} after {TimespanTotalSeconds} seconds due to {ExceptionMessage}", 
                        attempt, timespan.TotalSeconds, outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
                });
    }
}