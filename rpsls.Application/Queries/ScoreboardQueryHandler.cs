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
        var dbResults=  await scoreboardRepo.GetRecentResults(request.Count, ct);
        
        return dbResults.Select(r => r.ToResultWithTimeDto(timeService));
    }
}