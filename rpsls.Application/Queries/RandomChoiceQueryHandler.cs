using MediatR;
using rpsls.Application.DTOs;
using rpsls.Application.Interfaces;
using rpsls.Domain.Interfaces;

namespace rpsls.Application.Queries;

public class RandomChoiceQueryHandler(IGameService gameService, IRandomNumberProvider rnProvider) : IRequestHandler<RandomChoiceQuery, ChoiceDto>
{
    public async Task<ChoiceDto> Handle(RandomChoiceQuery request, CancellationToken ct)
    {
        var randomNumber = await rnProvider.GetRandomNumber(ct);
        var choice = await gameService.MapNumberToChoice(randomNumber);
        
        return new ChoiceDto((int)choice, choice.ToString());
    }
}