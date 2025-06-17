using rpsls.Domain.Models;

namespace rpsls.Application.Interfaces;

public interface IScoreboardRepository
{
    Task AddResult(string username, Choice playerChoice, Choice computerChoice, Outcome outcome, CancellationToken ct = default);
    Task<IEnumerable<GameResult>> GetRecentResults(int count = 10, CancellationToken ct = default);
    Task ResetScoreboard(CancellationToken ct = default);
}