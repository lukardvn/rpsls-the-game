using Moq;
using rpsls.Application.Interfaces;
using rpsls.Application.Queries;
using rpsls.Domain.Models;

namespace rpsls.Application.Tests.Queries;

public class ScoreboardQueryHandlerTests
{
    private readonly Mock<IScoreboardRepository> _repoMock = new();
    private readonly Mock<ITimeService> _timeServiceMock = new();
    private readonly ScoreboardQueryHandler _handler;

    public ScoreboardQueryHandlerTests()
    {
        _handler = new ScoreboardQueryHandler(_repoMock.Object, _timeServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsMappedResults()
    {
        // Arrange
        var utcTime = new DateTime(2024, 6, 18, 10, 0, 0, DateTimeKind.Utc);
        var localTime = new DateTime(2024, 6, 18, 12, 0, 0); // Assume UTC+2
        var query = new ScoreboardQuery("User1", 3);
        var mockResults = new List<GameResult>
        {
            new(Guid.NewGuid(), "User1", Choice.Rock, Choice.Scissors, Outcome.Win, DateTime.UtcNow),
            new(Guid.NewGuid(), "User1", Choice.Paper, Choice.Rock, Outcome.Win, DateTime.UtcNow.AddMinutes(-10))
        };

        _repoMock
            .Setup(repo => repo.GetRecentResults("User1", query.Count, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResults);

        _timeServiceMock
            .Setup(t => t.ConvertUtcToBelgradeTime(utcTime))
            .Returns(localTime);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var list = result.ToList();
        Assert.Equal(mockResults.Count, list.Count);
        _repoMock.Verify(r => r.GetRecentResults("User1", 3, It.IsAny<CancellationToken>()), Times.Once);
        _timeServiceMock.Verify(t => t.ConvertUtcToBelgradeTime(It.IsAny<DateTime>()), Times.Exactly(mockResults.Count));
    }
}