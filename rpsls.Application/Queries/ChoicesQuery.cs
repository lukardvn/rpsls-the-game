using MediatR;
using rpsls.Application.DTOs;

namespace rpsls.Application.Queries;

public record ChoicesQuery() : IRequest<IEnumerable<ChoiceDto>>;