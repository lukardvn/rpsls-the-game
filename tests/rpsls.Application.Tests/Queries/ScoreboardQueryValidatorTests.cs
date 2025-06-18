using rpsls.Application.Queries;

namespace rpsls.Application.Tests.Queries;

public class ScoreboardQueryValidatorTests
{
    private readonly ScoreboardQueryValidator _validator = new();

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void Validate_WhenValidCount_ShouldSucceed(int validCount)
    {
        // Arrange
        var query = new ScoreboardQuery { Count = validCount };

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(0, "Count must be greater than 0")]
    [InlineData(-10, "Count must be greater than 0")]
    [InlineData(101, "Count cannot exceed 100")]
    public void Validate_WhenInvalidCount_ShouldFail(int count, string expectedMessage)
    {
        // Arrange
        var query = new ScoreboardQuery { Count = count };

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Count", result.Errors[0].PropertyName);
        Assert.Equal(expectedMessage, result.Errors[0].ErrorMessage);
    }
}