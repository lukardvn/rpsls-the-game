using MediatR;
using rpsls.Application.Interfaces;

namespace rpsls.Application.Commands;

public class ResetScoreboardCommandHandler(IScoreboardRepository repo)
    : IRequestHandler<ResetScoreboardCommand>
{
    public async Task Handle(ResetScoreboardCommand request, CancellationToken ct) 
        => await repo.ResetScoreboard(ct);
}
