using Microsoft.Extensions.Logging;
using rpsls.Domain.Interfaces;
using rpsls.Domain.Models;

namespace rpsls.Domain.Services;

public class GameService(ILogger<GameService> logger) : IGameService
{
    private readonly Dictionary<Choice, Choice[]> _winningRules = new()
    {
        { Choice.Rock, [Choice.Scissors, Choice.Lizard] },
        { Choice.Paper, [Choice.Rock, Choice.Spock] },
        { Choice.Scissors, [Choice.Paper, Choice.Lizard] },
        { Choice.Lizard, [Choice.Spock, Choice.Paper] },
        { Choice.Spock, [Choice.Scissors, Choice.Rock] }
    };
    
    public Task<Choice> MapNumberToChoice(int number)
    {
        logger.LogInformation("Mapping random number (1-100) to one of the choices.");
        
        if (number is < 1 or > 100)
            throw new ArgumentOutOfRangeException(nameof(number), "Number must be between 1 and 100.");

        var choices = Enum.GetValues<Choice>();
        var bucketSize = 100 / choices.Length;
        var index = Math.Min((number - 1) / bucketSize, choices.Length - 1);
        
        return Task.FromResult(choices[index]);
    }

    public Task<Outcome> DetermineOutcome(Choice player, Choice computer)
    {
        logger.LogInformation("Let's see who wins!");

        if (player == computer)
            return Task.FromResult(Outcome.Tie);

        var outcome = Task.FromResult(_winningRules[player].Contains(computer) 
            ? Outcome.Win 
            : Outcome.Lose);

        switch (outcome.Result)
        {
            case Outcome.Win:
                logger.LogInformation("Player won against the computer.");
                break;
            case Outcome.Lose:
                logger.LogInformation("Computer won against the player.");
                break;
            case Outcome.Tie:
            default:
                logger.LogInformation("Player and computer played the same hand.");
                break;
        }
        
        return outcome;
    }
}