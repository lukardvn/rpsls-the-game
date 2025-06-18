using MediatR;
using rpsls.Application.DTOs;
using rpsls.Application.Helpers;
using rpsls.Application.Interfaces;

namespace rpsls.Application.Queries;

public class ScoreboardQueryHandler(IScoreboardRepository scoreboardRepo, ITimeService timeService)
    : IRequestHandler<ScoreboardQuery, IEnumerable<ResultWithTimeDto>>
{
    public async Task<IEnumerable<ResultWithTimeDto>> Handle(ScoreboardQuery request, CancellationToken ct)
    {
        var gameResults=  await scoreboardRepo.GetRecentResults(request.Username, request.Count, ct);
        
        return gameResults.Select(r => r.ToResultWithTimeDto(timeService));
    }
}