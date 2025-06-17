using Microsoft.EntityFrameworkCore;
using rpsls.Application.Interfaces;
using rpsls.Domain.Models;
using rpsls.Infrastructure.Database;

namespace rpsls.Infrastructure.Repositories;

public class ScoreboardRepository(ApplicationDbContext dbContext) : IScoreboardRepository
{
    public async Task AddResult(string username, Choice playerChoice, Choice computerChoice, Outcome outcome, CancellationToken ct = default)
    {
        var result = new GameResult
        {
            Username = username,
            PlayerChoice = playerChoice,
            ComputerChoice = computerChoice,
            Outcome = outcome,
            PlayedAt = DateTime.UtcNow
        };
        
        dbContext.Results.Add(result);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<GameResult>> GetRecentResults(int count = 10, CancellationToken ct = default)
    {
        return await dbContext.Results
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
    /// </summary>
    /// <param name="ct"></param>
    public async Task ResetScoreboard(CancellationToken ct = default)
    {
        var resultsToArchive = await dbContext.Results.ToListAsync(ct);

        var archivedResults = resultsToArchive.Select(r => new ArchivedGameResult
        {
            Username = r.Username,
            PlayerChoice = r.PlayerChoice,
            ComputerChoice = r.ComputerChoice,
            Outcome = r.Outcome,
            PlayedAt = r.PlayedAt,
            ArchivedAt = DateTime.UtcNow
        });
        
        await dbContext.ArchivedResults.AddRangeAsync(archivedResults, ct);
        dbContext.Results.RemoveRange(resultsToArchive);
        
        await dbContext.SaveChangesAsync(ct);
    }
}