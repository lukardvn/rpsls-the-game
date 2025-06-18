using MediatR;
using rpsls.Application.DTOs;
using rpsls.Application.Interfaces;

namespace rpsls.Application.Queries;

public class LeaderboardQueryHandler(IScoreboardRepository repository): IRequestHandler<LeaderboardQuery, IEnumerable<LeaderboardEntryDto>>
{
    public async Task<IEnumerable<LeaderboardEntryDto>> Handle(LeaderboardQuery request, CancellationToken ct) 
        => await repository.GetTopRatedPlayers(request.Count, ct);
}