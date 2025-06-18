using Moq;
using rpsls.Application.Interfaces;
using rpsls.Application.Queries;
using rpsls.Domain.Interfaces;
using rpsls.Domain.Models;

namespace rpsls.Application.Tests.Queries;

public class RandomChoiceQueryHandlerTests
{
    private readonly Mock<IGameService> _gameServiceMock = new();
    private readonly Mock<IRandomNumberProvider> _rnProviderMock = new();
    private readonly RandomChoiceQueryHandler _handler;

    public RandomChoiceQueryHandlerTests()
    {
        _handler = new RandomChoiceQueryHandler(_gameServiceMock.Object, _rnProviderMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedChoice()
    {
        // Arrange
        const int number = 3;
        const Choice expectedChoice = Choice.Scissors;

        _rnProviderMock.Setup(x => x.GetRandomNumber(It.IsAny<CancellationToken>()))
            .ReturnsAsync(number);

        _gameServiceMock.Setup(x => x.MapNumberToChoice(number))
            .ReturnsAsync(expectedChoice);

        // Act
        var result = await _handler.Handle(new RandomChoiceQuery(), CancellationToken.None);

        // Assert
        Assert.Equal((int)expectedChoice, result.Id);
        Assert.Equal(expectedChoice.ToString(), result.Name);
    }
}