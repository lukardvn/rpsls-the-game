using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using rpsls.Infrastructure.DTOs;
using rpsls.Infrastructure.Services;

namespace rpsls.Test.UnitTests;

public class RandomNumberProviderTests
{
    [Fact]
    public async Task GetRandomNumber_ReturnsNumberFromApi_WhenResponseIsValid()
    {
        // Arrange
        const int expectedNumber = 42;
        var response = new RandomNumberResponse( expectedNumber);

        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(response),
            })
            .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object);
        var loggerMock = new Mock<ILogger<RandomNumberProvider>>();

        var provider = new RandomNumberProvider(httpClient, loggerMock.Object);

        // Act
        var actual = await provider.GetRandomNumber();

        // Assert
        Assert.Equal(expectedNumber, actual);
        handlerMock.Protected().Verify(
            "SendAsync", 
            Times.Once(), 
            ItExpr.IsAny<HttpRequestMessage>(), 
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetRandomNumber_ReturnsLocalRandom_WhenApiResponseIsNull()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create<object>(null!), // simulate null JSON response
            })
            .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object);
        var loggerMock = new Mock<ILogger<RandomNumberProvider>>();

        var provider = new RandomNumberProvider(httpClient, loggerMock.Object);

        // Act
        var actual = await provider.GetRandomNumber();

        // Assert
        Assert.InRange(actual, 1, 100); // local fallback returns 1-99
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Received null response")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}