using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using rpsls.Infrastructure.DTOs;
using rpsls.Infrastructure.Services;

namespace rpsls.Infrastructure.Tests;

public class RandomNumberProviderTests
{
    private readonly Mock<ILogger<RandomNumberProvider>> _loggerMock = new();
    
    [Fact]
    public async Task GetRandomNumber_WhenValidResponse_ShouldReturnApiNumberAndLogInfo()
    {
        // Arrange
        const int validNumber = 42;
        var apiResponse = new RandomNumberResponse(validNumber);
        var httpClient = CreateHttpClientMock(apiResponse);
        var service = CreateService(httpClient);

        // Act
        var result = await service.GetRandomNumber();

        // Assert
        Assert.Equal(validNumber, result);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains($"Received random number: {validNumber}", StringComparison.CurrentCultureIgnoreCase)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }

    
    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    [InlineData(-5)]
    public async Task GetRandomNumber_WhenInvalidResponse_ShouldFallbackAndLogError(int invalidNumber)
    {
        // Arrange
        var httpClient = CreateHttpClientThrowing();
        var loggerMock = new Mock<ILogger<RandomNumberProvider>>();
        var service = new RandomNumberProvider(httpClient, loggerMock.Object);

        // Act
        var result = await service.GetRandomNumber();

        // Assert fallback behavior
        Assert.InRange(result, 1, 100);
        Assert.NotEqual(invalidNumber, result);

        // Assert the warning log was called at least once
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("exception occurred while fetching random number", StringComparison.CurrentCultureIgnoreCase)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    [InlineData(-5)]
    public async Task GetRandomNumber_WhenApiReturnsInvalidNumber_ShouldFallbackAndLogWarning(int invalidNumber)
    {
        // Arrange
        var httpClient = CreateHttpClientMock(new RandomNumberResponse(invalidNumber));
        var service = CreateService(httpClient);

        // Act
        var result = await service.GetRandomNumber();

        // Assert
        Assert.InRange(result, 1, 100);
        Assert.NotEqual(invalidNumber, result);

        // Verify warning log called with partial message
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => 
                    v.ToString()!.Contains("invalid or null random number", StringComparison.OrdinalIgnoreCase)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    
    private RandomNumberProvider CreateService(HttpClient httpClient) => new(httpClient, _loggerMock.Object);

    private static HttpClient CreateHttpClientMock(RandomNumberResponse? apiResponse)
    {
        var responseMessage = apiResponse == null
            ? new HttpResponseMessage(HttpStatusCode.OK) { Content = null }
            : new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(apiResponse) };

        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage)
            .Verifiable();

        var client = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://dummy-base-url.com/")
        };
        return client;
    }

    private static HttpClient CreateHttpClientThrowing()
    {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"))
            .Verifiable();

        return new HttpClient(handlerMock.Object);
    }
}