using Moq;
using rpsls.Application.Commands;
using rpsls.Application.Interfaces;

namespace rpsls.Application.Tests.Commands;


public class ResetScoreboardCommandHandlerTests
{
    private readonly Mock<IScoreboardRepository> _repoMock = new();
    private readonly ResetScoreboardCommandHandler _handler;

    public ResetScoreboardCommandHandlerTests()
    {
        _handler = new ResetScoreboardCommandHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_CallsResetScoreboardOnce()
    {
        // Arrange
        var command = new ResetScoreboardCommand();
        var cancellationToken = CancellationToken.None;

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _repoMock.Verify(x => x.ResetScoreboard(cancellationToken), Times.Once);
    }
}