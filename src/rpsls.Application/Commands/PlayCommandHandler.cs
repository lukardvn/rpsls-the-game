using MediatR;
using rpsls.Application.DTOs;
using rpsls.Application.Interfaces;
using rpsls.Domain.Interfaces;
using rpsls.Domain.Models;

namespace rpsls.Application.Commands;

public class PlayCommandHandler(IGameService gameService, IRandomNumberProvider rnProvider)
    : IRequestHandler<PlayCommand, ResultDto>
{
    public async Task<ResultDto> Handle(PlayCommand request, CancellationToken ct)
    {
        var playerChoice = (Choice)request.Player;

        var computerNumber = await rnProvider.GetRandomNumber(ct);
        var computerChoice = await gameService.MapNumberToChoice(computerNumber);

        var result = await gameService.DetermineOutcome(playerChoice, computerChoice);

        return new ResultDto(
            playerChoice,
            computerChoice,
            result
        );
    }
}