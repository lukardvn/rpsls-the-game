using Microsoft.Extensions.Logging;
using rpsls.Application.Interfaces;

namespace rpsls.Infrastructure.Services;

public class TimeService(ILogger<TimeService> logger) : ITimeService
{
    public DateTime ConvertUtcToBelgradeTime(DateTime utcInput)
    {
        logger.LogInformation("Converting utc time stored in database to local Belgrade time.");
        
        var timeZoneId = OperatingSystem.IsWindows() ? "Central European Standard Time" : "Europe/Belgrade";
        var belgradeTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return TimeZoneInfo.ConvertTimeFromUtc(utcInput, belgradeTimeZone);
    }
}