using MediatR;
using rpsls.Application.DTOs;

namespace rpsls.Application.Queries;

public record LeaderboardQuery(int Count = 10) : IRequest<IEnumerable<LeaderboardEntryDto>>;