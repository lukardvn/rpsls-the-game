using Microsoft.EntityFrameworkCore;
using rpsls.Domain.Models;
using rpsls.Infrastructure.Database;
using rpsls.Infrastructure.Repositories;

namespace rpsls.Infrastructure.Tests;

public class ScoreboardRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ScoreboardRepository _repository;

    public ScoreboardRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _repository = new ScoreboardRepository(_dbContext);
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task AddResult_ShouldAddRecordToDb()
    {
        await _repository.AddResult("user1", Choice.Rock, Choice.Paper, Outcome.Lose);

        var results = await _dbContext.Results.ToListAsync();
        Assert.Single(results);
        Assert.Equal("user1", results[0].Username);
        Assert.Equal(Choice.Rock, results[0].PlayerChoice);
        Assert.Equal(Outcome.Lose, results[0].Outcome);
    }

    [Fact]
    public async Task GetRecentResults_ShouldReturnRecentRecordsForUser()
    {
        // Arrange
        for (var i = 0; i < 5; i++)
        {
            _dbContext.Results.Add(new GameResult(Guid.NewGuid(), "user1", Choice.Paper, Choice.Rock, Outcome.Win,
                DateTime.UtcNow.AddSeconds(i)));
        }

        for (var i = 0; i < 10; i++)
        {
            _dbContext.Results.Add(new GameResult(Guid.NewGuid(), $"user{i + 2}", Choice.Scissors, Choice.Paper, Outcome.Lose,
                DateTime.UtcNow.AddSeconds(i)));
        }

        await _dbContext.SaveChangesAsync();

        // Act
        var recentResults = await _repository.GetRecentResults(username: "user1");

        // Assert
        var gameResults = recentResults as GameResult[] ?? recentResults.ToArray();

        Assert.All(gameResults, gr => Assert.Equal("user1", gr.Username));
        Assert.Equal(5, gameResults.Length);
        Assert.True(gameResults[0].PlayedAt >= gameResults[1].PlayedAt);
    }

    [Fact]
    public async Task GetRecentResults_ShouldRespectCountLimit()
    {
        for (var i = 0; i < 20; i++)
        {
            _dbContext.Results.Add(new GameResult(Guid.NewGuid(), "user1", Choice.Rock, Choice.Scissors, Outcome.Win, DateTime.UtcNow.AddMinutes(-i)));
        }
        await _dbContext.SaveChangesAsync();

        var recentResults = await _repository.GetRecentResults("user1", count: 5);

        Assert.Equal(5, recentResults.Count());
    }

    [Fact]
    public async Task GetRecentResults_ShouldReturnEmpty_WhenUserDoesNotExist()
    {
        var results = await _repository.GetRecentResults("non-existing-user");

        Assert.Empty(results);
    }
    
    [Fact]
    public async Task ResetScoreboard_ShouldMoveResultsToArchiveAndRemoveFromResults()
    {
        // Arrange
        _dbContext.Results.Add(new GameResult(Guid.NewGuid(), "userToArchive", Choice.Spock, Choice.Lizard, Outcome.Win, DateTime.UtcNow.AddMinutes(-5)));
        await _dbContext.SaveChangesAsync();

        // Act
        await _repository.ResetScoreboard();

        // Assert
        var resultsCount = await _dbContext.Results.CountAsync();
        var archivedCount = await _dbContext.ArchivedResults.CountAsync();

        Assert.Equal(0, resultsCount);
        Assert.Equal(1, archivedCount);

        var archived = await _dbContext.ArchivedResults.FirstAsync();
        Assert.Equal("userToArchive", archived.Username);
        Assert.Equal(Choice.Spock, archived.PlayerChoice);
        Assert.Equal(Outcome.Win, archived.Outcome);
    }
    
    [Fact]
    public async Task GetTopRatedPlayers_ShouldReturnSortedLeaderboard()
    {
        _dbContext.Results.AddRange(
            new GameResult(Guid.NewGuid(), "user1", Choice.Rock, Choice.Scissors, Outcome.Win, DateTime.UtcNow),
            new GameResult(Guid.NewGuid(), "user1", Choice.Paper, Choice.Rock, Outcome.Lose, DateTime.UtcNow),
            new GameResult(Guid.NewGuid(), "user2", Choice.Scissors, Choice.Paper, Outcome.Win, DateTime.UtcNow),
            new GameResult(Guid.NewGuid(), "user2", Choice.Lizard, Choice.Spock, Outcome.Win, DateTime.UtcNow)
        );

        await _dbContext.SaveChangesAsync();

        var leaderboard = await _repository.GetTopRatedPlayers(5);
        var entries = leaderboard.ToList();

        Assert.Equal(2, entries.Count);
        Assert.Equal("user2", entries[0].Username);
        Assert.Equal(100.0, entries[0].WinRate);
        Assert.Equal(50.0, entries[1].WinRate);
    }

    [Fact]
    public async Task GetTopRatedPlayers_WhenNoResults_ShouldReturnEmpty()
    {
        var leaderboard = await _repository.GetTopRatedPlayers(10);
        Assert.Empty(leaderboard);
    }
    [Fact]
    public async Task GetTopRatedPlayers_ShouldCalculateZeroWinRate_WhenAllLosses()
    {
        _dbContext.Results.AddRange(
            new GameResult(Guid.NewGuid(), "loser", Choice.Spock, Choice.Lizard, Outcome.Lose, DateTime.UtcNow),
            new GameResult(Guid.NewGuid(), "loser", Choice.Paper, Choice.Rock, Outcome.Lose, DateTime.UtcNow)
        );

        await _dbContext.SaveChangesAsync();

        var leaderboard = await _repository.GetTopRatedPlayers(5);
        var entry = leaderboard.First();

        Assert.Equal("loser", entry.Username);
        Assert.Equal(0, entry.Wins);
        Assert.Equal(2, entry.TotalGames);
        Assert.Equal(0.0, entry.WinRate);
    }
}