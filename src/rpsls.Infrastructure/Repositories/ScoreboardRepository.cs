using Microsoft.EntityFrameworkCore;
using rpsls.Application.DTOs;
using rpsls.Application.Interfaces;
using rpsls.Domain.Models;
using rpsls.Infrastructure.Database;

namespace rpsls.Infrastructure.Repositories;

public class ScoreboardRepository(ApplicationDbContext dbContext) : IScoreboardRepository
{
    /// <summary>
    /// Adding result of the game to the database table.
    /// Assumes username has already been validated (not null/empty) by the caller.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="playerChoice"></param>
    /// <param name="computerChoice"></param>
    /// <param name="outcome"></param>
    /// <param name="ct"></param>
    public async Task AddResult(string username, Choice playerChoice, Choice computerChoice, Outcome outcome, CancellationToken ct = default)
    {
        var result = new GameResult(
            Guid.Empty, 
            username, 
            playerChoice, 
            computerChoice, 
            outcome, 
            DateTime.UtcNow
        );
        
        dbContext.Results.Add(result);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<GameResult>> GetRecentResults(string username, int count = 10, CancellationToken ct = default)
    {
        return await dbContext.Results
            .Where(r => r.Username.ToLower().Equals(username.ToLower()))
            .OrderByDescending(r => r.PlayedAt)
            .Take(count)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Resetting scoreboard this way in order to show how we would handle potential analytics or something similar.
    /// Never actually deleting the records in a database.
    /// Keeping the 'result' table clean in order to avoid additional conditioning and improve performance when fetching data.
    ///
    /// One more alternative way (simpler) would be to introduce a field (IsArchived) that would act as a flag.
    /// And the obvious way would be to just drop the entire table.
    /// </summary>
    /// <param name="ct"></param>
    public async Task ResetScoreboard(CancellationToken ct = default)
    {
        var resultsToArchive = await dbContext.Results.ToListAsync(ct);

        var archivedResults = resultsToArchive.Select(r =>
            new ArchivedGameResult(
                Guid.Empty, //EF generates the GUID                  
                r.Username,
                r.PlayerChoice,
                r.ComputerChoice,
                r.Outcome,
                r.PlayedAt,
                DateTime.UtcNow
            )
        );
        
        await dbContext.ArchivedResults.AddRangeAsync(archivedResults, ct);
        dbContext.Results.RemoveRange(resultsToArchive);
        
        await dbContext.SaveChangesAsync(ct);
    }
    
    /// <summary>
    /// Retrieves the top-rated players based on their number of wins and win rate.
    /// Players are ranked primarily by number of wins, and then by win rate if tied.
    /// </summary>
    /// <param name="count"></param>
    /// <param name="ct"></param>
    public async Task<IEnumerable<LeaderboardEntryDto>> GetTopRatedPlayers(int count, CancellationToken ct = default)
    {
        var rawData = await dbContext.Results
            .Where(r => !string.IsNullOrEmpty(r.Username))
            .GroupBy(r => r.Username)
            .Select(g => new
            {
                Username = g.Key,
                Wins = g.Count(r => r.Outcome == Outcome.Win),
                TotalGames = g.Count()
            })
            .ToListAsync(ct);

        var leaderboard = rawData
            .Select(x => new LeaderboardEntryDto(
                x.Username,
                x.Wins,
                x.TotalGames,
                x.TotalGames > 0
                    ? Math.Round((double)x.Wins / x.TotalGames * 100, 2)
                    : 0.0
            ))
            .OrderByDescending(x => x.Wins)
            .ThenByDescending(x => x.WinRate)
            .Take(count);

        return leaderboard;
    }
}