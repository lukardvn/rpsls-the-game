namespace rpsls.Application.Interfaces;

public interface ITimeService
{
    DateTime ConvertUtcToBelgradeTime(DateTime utcInput);
}