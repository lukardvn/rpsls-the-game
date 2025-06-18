using rpsls.Domain.Models;
using rpsls.Domain.Services;
using Xunit;

namespace rpsls.Domain.Tests;



public class GameServiceTests
{
    private readonly GameService _service = new(null!);

    [Theory]
    [InlineData(20, Choice.Rock)]
    [InlineData(60, Choice.Scissors)]
    [InlineData(61, Choice.Lizard)]
    [InlineData(100, Choice.Spock)]
    public async Task MapNumberToChoice_WhenValidNumber_ShouldReturnExpectedChoice(int number, Choice expectedChoice)
    {
        var choice = await _service.MapNumberToChoice(number);
        Assert.Equal(choice, expectedChoice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(101)]
    public async Task MapNumberToChoice_WhenInvalidNumber_ShouldThrowException(int invalidNumber)
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        {
            await _service.MapNumberToChoice(invalidNumber);
        });
    }

    [Theory]
    [InlineData(Choice.Rock, Choice.Rock, Outcome.Tie)]
    [InlineData(Choice.Paper, Choice.Paper, Outcome.Tie)]
    [InlineData(Choice.Spock, Choice.Spock, Outcome.Tie)]
    public async Task DetermineOutcome_WhenSameChoice_ShouldReturnTie(Choice player, Choice computer, Outcome expected)
    {
        var result = await _service.DetermineOutcome(player, computer);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(Choice.Rock, Choice.Scissors)]
    [InlineData(Choice.Paper, Choice.Rock)]
    [InlineData(Choice.Scissors, Choice.Paper)]
    [InlineData(Choice.Lizard, Choice.Spock)]
    [InlineData(Choice.Spock, Choice.Rock)]
    public async Task DetermineOutcome_WhenPlayerBeatsComputer_ShouldReturnWin(Choice player, Choice computer)
    {
        var result = await _service.DetermineOutcome(player, computer);
        Assert.Equal(Outcome.Win, result);
    }

    [Theory]
    [InlineData(Choice.Rock, Choice.Paper)]
    [InlineData(Choice.Paper, Choice.Scissors)]
    [InlineData(Choice.Scissors, Choice.Rock)]
    [InlineData(Choice.Lizard, Choice.Rock)]
    [InlineData(Choice.Spock, Choice.Lizard)]
    public async Task DetermineOutcome_WhenPlayerLosesToComputer_ShouldReturnLose(Choice player, Choice computer)
    {
        var result = await _service.DetermineOutcome(player, computer);
        Assert.Equal(Outcome.Lose, result);
    }
}