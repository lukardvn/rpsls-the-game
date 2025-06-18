using rpsls.Application.DTOs;
using rpsls.Domain.Models;

namespace rpsls.Application.Interfaces;

public interface IScoreboardRepository
{
    Task AddResult(string username, Choice playerChoice, Choice computerChoice, Outcome outcome, CancellationToken ct = default);
    Task<IEnumerable<GameResult>> GetRecentResults(string username, int count, CancellationToken ct = default);
    Task ResetScoreboard(CancellationToken ct = default);
    Task<IEnumerable<LeaderboardEntryDto>> GetTopRatedPlayers(int count, CancellationToken ct = default);
}