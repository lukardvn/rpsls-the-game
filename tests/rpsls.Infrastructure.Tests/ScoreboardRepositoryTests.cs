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
    public async Task GetRecentResults_ShouldReturnRecentRecords_ForSpecificUser()
    {
        // Arrange
        for (var i = 0; i < 5; i++)
        {
            _dbContext.Results.Add(new GameResult
            {
                Username = "user1",
                PlayerChoice = Choice.Paper,
                ComputerChoice = Choice.Rock,
                Outcome = Outcome.Win,
                PlayedAt = DateTime.UtcNow.AddSeconds(i)
            });
        }

        for (var i = 0; i < 10; i++)
        {
            _dbContext.Results.Add(new GameResult
            {
                Username = $"user{i + 2}",
                PlayerChoice = Choice.Scissors,
                ComputerChoice = Choice.Paper,
                Outcome = Outcome.Lose,
                PlayedAt = DateTime.UtcNow.AddSeconds(i)
            });
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
    public async Task ResetScoreboard_ShouldMoveResultsToArchiveAndRemoveFromResults()
    {
        // Arrange
        _dbContext.Results.Add(new GameResult
        {
            Username = "userToArchive",
            PlayerChoice = Choice.Spock,
            ComputerChoice = Choice.Lizard,
            Outcome = Outcome.Win,
            PlayedAt = DateTime.UtcNow.AddHours(-1)
        });
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
}
