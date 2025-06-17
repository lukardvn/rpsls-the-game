using rpsls.Domain.Models;

namespace rpsls.Domain.Interfaces;

//here instead of Application layer because of circular dependency
public interface IGameService
{
    Task<Choice> MapNumberToChoice(int number);
    Task<Outcome> DetermineOutcome(Choice player, Choice computer);
}