using rpsls.Domain.Models;
using rpsls.Domain.Services;

namespace rpsls.Test.UnitTests;

public class GameServiceTests
{
    private readonly GameService _gameService = new(null!);
    
    [Theory]
    [InlineData(1, Choice.Rock)]
    [InlineData(20, Choice.Rock)]
    [InlineData(21, Choice.Paper)]
    [InlineData(40, Choice.Paper)]
    [InlineData(41, Choice.Scissors)]
    [InlineData(60, Choice.Scissors)]
    [InlineData(61, Choice.Lizard)]
    [InlineData(80, Choice.Lizard)]
    [InlineData(81, Choice.Spock)]
    [InlineData(100, Choice.Spock)]
    public async Task MapNumberToChoice_ReturnsCorrectChoice(int input, Choice expected)
    {
        var result = await _gameService.MapNumberToChoice(input);
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData(Choice.Rock, Choice.Scissors, Outcome.Win)]
    [InlineData(Choice.Rock, Choice.Lizard, Outcome.Win)]
    [InlineData(Choice.Rock, Choice.Paper, Outcome.Lose)]
    [InlineData(Choice.Paper, Choice.Spock, Outcome.Win)]
    [InlineData(Choice.Spock, Choice.Spock, Outcome.Tie)]
    [InlineData(Choice.Lizard, Choice.Scissors, Outcome.Lose)]
    public async Task DetermineOutcome_ReturnsCorrectOutcome(Choice player, Choice computer, Outcome expectedOutcome)
    {
        var result = await _gameService.DetermineOutcome(player, computer);
        Assert.Equal(expectedOutcome, result);
    }
}