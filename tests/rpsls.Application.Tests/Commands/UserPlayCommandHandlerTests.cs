using Moq;
using rpsls.Application.Commands;
using rpsls.Application.Interfaces;
using rpsls.Domain.Interfaces;
using rpsls.Domain.Models;

namespace rpsls.Application.Tests.Commands;

public class UserPlayCommandHandlerTests
{
    private readonly Mock<IGameService> _gameServiceMock = new();
    private readonly Mock<IRandomNumberProvider> _randomNumberProviderMock = new();
    private readonly Mock<IScoreboardRepository> _scoreboardRepoMock = new();

    private readonly UserPlayCommandHandler _handler;

    public UserPlayCommandHandlerTests()
    {
        _handler = new UserPlayCommandHandler(
            _gameServiceMock.Object,
            _randomNumberProviderMock.Object,
            _scoreboardRepoMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenUsernamePassedIn_ShouldSaveResultAndReturnDto()
    {
        // Arrange
        var command = new UserPlayCommand((int)Choice.Rock, "TestUser");
        const int computerNumber = 2;
        const Choice computerChoice = Choice.Paper;
        const Outcome outcome = Outcome.Lose;

        _randomNumberProviderMock.Setup(x => x.GetRandomNumber(It.IsAny<CancellationToken>()))
            .ReturnsAsync(computerNumber);

        _gameServiceMock.Setup(x => x.MapNumberToChoice(computerNumber))
            .ReturnsAsync(computerChoice);

        _gameServiceMock.Setup(x => x.DetermineOutcome(Choice.Rock, computerChoice))
            .ReturnsAsync(outcome);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Choice.Rock, result.Player);
        Assert.Equal(computerChoice, result.Computer);
        Assert.Equal(outcome, result.Result);

        _scoreboardRepoMock.Verify(x =>
            x.AddResult("TestUser", Choice.Rock, computerChoice, outcome, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_WhenUsernameNotPassedIn_ShouldNotSaveResult(string? username)
    {
        // Arrange
        var command = new UserPlayCommand((int)Choice.Spock, username);
        const int computerNumber = 1;
        const Choice computerChoice = Choice.Lizard;
        const Outcome outcome = Outcome.Win;

        _randomNumberProviderMock.Setup(x => x.GetRandomNumber(It.IsAny<CancellationToken>()))
            .ReturnsAsync(computerNumber);

        _gameServiceMock.Setup(x => x.MapNumberToChoice(computerNumber))
            .ReturnsAsync(computerChoice);

        _gameServiceMock.Setup(x => x.DetermineOutcome(Choice.Spock, computerChoice))
            .ReturnsAsync(outcome);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Choice.Spock, result.Player);
        Assert.Equal(computerChoice, result.Computer);
        Assert.Equal(outcome, result.Result);

        _scoreboardRepoMock.Verify(x =>
            x.AddResult(It.IsAny<string>(), It.IsAny<Choice>(), It.IsAny<Choice>(), It.IsAny<Outcome>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
