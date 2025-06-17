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
    public async Task GetRandomNumber_ValidResponse_ReturnsApiNumber()
    {
        // Arrange
        const int number = 42;
        
        var httpClient = CreateHttpClientMock(new RandomNumberResponse(number));
        var service = CreateService(httpClient);

        // Act
        var result = await service.GetRandomNumber();

        // Assert
        Assert.Equal(number, result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    [InlineData(-5)]
    public async Task GetRandomNumber_InvalidApiNumber_FallsBackToRandom(int invalidNumber)
    {
        // Arrange
        var httpClient = CreateHttpClientMock(new RandomNumberResponse(invalidNumber));
        var service = CreateService(httpClient);

        // Act
        var result = await service.GetRandomNumber();

        // Assert
        Assert.InRange(result, 1, 100);
        Assert.NotEqual(invalidNumber, result);
    }

    [Fact]
    public async Task GetRandomNumber_NullResponse_FallsBackToRandom()
    {
        // Arrange
        var httpClient = CreateHttpClientMock(null);
        var service = CreateService(httpClient);

        // Act
        var result = await service.GetRandomNumber();

        // Assert
        Assert.InRange(result, 1, 100);
    }

    [Fact]
    public async Task GetRandomNumber_HttpException_FallsBackToRandom()
    {
        // Arrange
        var httpClient = CreateHttpClientThrowing();
        var service = CreateService(httpClient);

        // Act
        var result = await service.GetRandomNumber();

        // Assert
        Assert.InRange(result, 1, 100);
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

        return new HttpClient(handlerMock.Object);
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