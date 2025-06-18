using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Moq;
using rpsls.Api.DTOs;
using rpsls.Application.Commands;
using rpsls.Application.DTOs;
using rpsls.Application.Queries;
using rpsls.Domain.Models;

namespace rpsls.Api.Tests;

//TODO: more tests
public sealed class GameEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<ISender> _mediatorMock;

    public GameEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _mediatorMock = new Mock<ISender>();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing ISender registration and add our mock
                var serviceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ISender));
                if (serviceDescriptor != null)
                    services.Remove(serviceDescriptor);

                services.AddSingleton(_mediatorMock.Object);
            });
        });
    }

    [Fact]
    public async Task GetChoices_ShouldReturnOkWithChoices()
    {
        // Arrange
        var expectedChoices = new List<ChoiceDto>
        {
            new(1, "Rock"),
            new(2, "Paper"),
            new(3, "Scissors"),
            new(4, "Lizard"),
            new(5, "Spock"),
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ChoicesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedChoices);

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/game/choices");

        // Assert
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<ChoiceDto>>();
        Assert.NotNull(result);
        Assert.Equal(expectedChoices.Count, result.Count);
        Assert.Equal(expectedChoices[0].Name, result[0].Name);
    }

    [Fact]
    public async Task GetRandomChoice_ShouldReturnOkWithChoice()
    {
        // Arrange
        var expectedChoice = new ChoiceDto(1, "Rock");

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<RandomChoiceQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedChoice);

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/game/choice");

        // Assert
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ChoiceDto>();
        Assert.NotNull(result);
        Assert.Equal(expectedChoice.Name, result.Name);
    }

    [Fact]
    public async Task Play_WhenPlayRequestValid_ShouldReturnOkWithResult()
    {
        // Arrange
        var playRequest = new PlayRequest((int)Choice.Rock, "TestUser");
        var expectedResult = new ResultDto(Choice.Rock, Choice.Scissors, Outcome.Win);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<PlayCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/game/play", playRequest);

        // Assert
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ResultDto>();
        Assert.NotNull(result);
        Assert.Equal(expectedResult.Result, result.Result);
        Assert.Equal(expectedResult.Player, result.Player);
        Assert.Equal(expectedResult.Computer, result.Computer);
    }

    [Fact]
    public async Task GetScoreboard_ShouldReturnOkWithResults()
    {
        // Arrange
        const string username = "user1";
        var expectedResults = new List<ResultWithTimeDto>
        {
            new(Choice.Rock, Choice.Paper, Outcome.Lose, DateTime.UtcNow),
            new(Choice.Scissors, Choice.Rock, Outcome.Lose, DateTime.UtcNow.AddMinutes(-1))
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<ScoreboardQuery>(q => q.Count == 10), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResults);

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/game/scoreboard?username={username}");

        // Assert
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<ResultWithTimeDto>>();
        Assert.NotNull(result);
        Assert.Equal(expectedResults.Count, result.Count);
    }

    [Fact]
    public async Task GetScoreboard_WhenCustomCount_ShouldReturnOkWith_Count_Results()
    {
        // Arrange
        const int count = 5;
        const string username = "user1";
        var expectedResults = new List<ResultWithTimeDto>
        {
            new(Choice.Rock, Choice.Paper, Outcome.Lose, DateTime.UtcNow)
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<ScoreboardQuery>(q => q.Count == count), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResults);

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/game/scoreboard?username={username}&count={count}");

        // Assert
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<ResultWithTimeDto>>();
        Assert.NotNull(result);
        Assert.Equal(expectedResults.Count, result.Count);
    }

    [Fact]
    public async Task ResetScoreboard_ShouldReturnOkWithMessage()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ResetScoreboardCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsync("/game/scoreboard/reset", null);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}