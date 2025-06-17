using rpsls.Domain.Interfaces;
using rpsls.Domain.Models;

namespace rpsls.Domain.Services;

public class GameService : IGameService
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
        var choices = Enum.GetValues<Choice>();
        var bucketSize = 100 / choices.Length;
        var index = Math.Min((number - 1) / bucketSize, choices.Length - 1);
        
        return Task.FromResult(choices[index]);
    }

    public Task<Outcome> DetermineOutcome(Choice player, Choice computer)
    {
        if (player == computer)
            return Task.FromResult(Outcome.Tie);

        return Task.FromResult(_winningRules[player].Contains(computer) 
            ? Outcome.Win 
            : Outcome.Lose);
    }
}