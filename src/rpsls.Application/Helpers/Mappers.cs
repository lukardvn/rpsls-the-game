using rpsls.Application.DTOs;
using rpsls.Application.Interfaces;
using rpsls.Domain.Models;

namespace rpsls.Application.Helpers;

public static class Mappers
{
    public static ResultWithTimeDto ToResultWithTimeDto(this GameResult result, ITimeService timeService)
    {
        return new ResultWithTimeDto(
            result.PlayerChoice, 
            result.ComputerChoice, 
            result.Outcome, 
            timeService.ConvertUtcToBelgradeTime(result.PlayedAt)
        );
    }
}