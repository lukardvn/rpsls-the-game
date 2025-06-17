using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using rpsls.Application.Interfaces;
using rpsls.Infrastructure.DTOs;

namespace rpsls.Infrastructure.Services;

public class RandomNumberProvider(HttpClient httpClient, ILogger<RandomNumberProvider> logger) : IRandomNumberProvider
{
    private const string BaseUrl = "https://codechallenge.boohma.com/random";

    public async Task<int> GetRandomNumber(CancellationToken ct = default)
    {
        var response = await httpClient.GetFromJsonAsync<RandomNumberResponse>(BaseUrl, ct);

        if (response is not null)
        {
            logger.LogInformation("Received random number: {RandomNumber}", response.RandomNumber);
            return response.RandomNumber;
        }

        logger.LogWarning("Received null response from random number API. Falling back to local random generator.");
        return new Random().Next(1, 100);
    }
}