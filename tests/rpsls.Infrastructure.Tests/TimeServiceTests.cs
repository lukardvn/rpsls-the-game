using Microsoft.Extensions.Logging;
using Moq;
using rpsls.Infrastructure.Services;

namespace rpsls.Infrastructure.Tests;

public class TimeServiceTests
{
    private readonly Mock<ILogger<TimeService>> _loggerMock = new();
    private readonly TimeService _service;

    public TimeServiceTests()
    {
        _service = new TimeService(_loggerMock.Object);
    }

    [Theory]
    [InlineData("2024-01-15T12:00:00Z", "2024-01-15T13:00:00")] // Winter (UTC+1)
    [InlineData("2024-07-15T12:00:00Z", "2024-07-15T14:00:00")] // Summer (UTC+2)
    public void ConvertUtcToBelgradeTime_ShouldConvertCorrectly(string utcString, string expectedLocalString)
    {
        // Arrange
        var utc = DateTime.Parse(utcString, null, System.Globalization.DateTimeStyles.AdjustToUniversal);
        var expectedLocal = DateTime.Parse(expectedLocalString);

        // Act
        var result = _service.ConvertUtcToBelgradeTime(utc);

        // Assert
        Assert.Equal(expectedLocal, result);
    }
}