using MediatR;
using rpsls.Application.DTOs;

namespace rpsls.Application.Queries;

public record RandomChoiceQuery() : IRequest<ChoiceDto>;