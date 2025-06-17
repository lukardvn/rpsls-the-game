using MediatR;
using rpsls.Application.DTOs;
using rpsls.Application.Interfaces;
using rpsls.Domain.Interfaces;
using rpsls.Domain.Models;

namespace rpsls.Application.Commands;

public class UserPlayCommandHandler(IGameService gameService, IRandomNumberProvider rnProvider, IScoreboardRepository scoreboardRepo)
    : IRequestHandler<UserPlayCommand, ResultDto>
{
    public async Task<ResultDto> Handle(UserPlayCommand request, CancellationToken ct)
    {
        var playerChoice = (Choice)request.Choice;

        var computerNumber = await rnProvider.GetRandomNumber(ct);
        var computerChoice = await gameService.MapNumberToChoice(computerNumber);

        var outcome = await gameService.DetermineOutcome(playerChoice, computerChoice);

        if (!string.IsNullOrWhiteSpace(request.Username))   // username was provided, save result to database
            await scoreboardRepo.AddResult(request.Username, playerChoice, computerChoice, outcome, ct);

        return new ResultDto(
            playerChoice,
            computerChoice,
            outcome
        );
    }
}