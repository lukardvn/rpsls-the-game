using MediatR;
using rpsls.Application.DTOs;

namespace rpsls.Application.Queries;

public record ScoreboardQuery(string Username, int Count = 10) : IRequest<IEnumerable<ResultWithTimeDto>>;