using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using rpsls.Application.Interfaces;
using rpsls.Infrastructure.DTOs;

namespace rpsls.Infrastructure.Services;

/// <summary>
/// Service that gets random number from external service. If it's not successful, then it generates the random number by itself.
/// </summary>
/// <param name="httpClient"></param>
/// <param name="logger"></param>
public class RandomNumberProvider(HttpClient httpClient, ILogger<RandomNumberProvider> logger) : IRandomNumberProvider
{
    public async Task<int> GetRandomNumber(CancellationToken ct = default)
    {
        const string endpoint = "random";
        
        try
        {
            var response = await httpClient.GetFromJsonAsync<RandomNumberResponse>(endpoint, ct);

            if (response?.RandomNumber is >= 1 and <= 100)
            {
                logger.LogInformation("Received random number: {RandomNumber}", response.RandomNumber);
                return response.RandomNumber;
            }

            logger.LogWarning("Received invalid response from CodeChallenge API. Falling back to local random generator.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while fetching random number. Falling back to local random generator.");
        }

        return new Random().Next(1, 101);
    }

}