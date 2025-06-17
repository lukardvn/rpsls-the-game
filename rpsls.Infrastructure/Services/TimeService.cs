using rpsls.Application.Interfaces;

namespace rpsls.Infrastructure.Services;

public class TimeService : ITimeService
{
    public DateTime ConvertUtcToBelgradeTime(DateTime utcInput)
    {
        var timeZoneId = OperatingSystem.IsWindows() ? "Central European Standard Time" : "Europe/Belgrade";
        var belgradeTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return TimeZoneInfo.ConvertTimeFromUtc(utcInput, belgradeTimeZone);
    }
}