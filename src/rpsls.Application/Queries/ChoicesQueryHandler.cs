using MediatR;
using rpsls.Application.DTOs;
using rpsls.Domain.Models;

namespace rpsls.Application.Queries;

public class ChoicesQueryHandler : IRequestHandler<ChoicesQuery, IEnumerable<ChoiceDto>>
{
    public Task<IEnumerable<ChoiceDto>> Handle(ChoicesQuery request, CancellationToken cancellationToken)
    {
        var choices = Enum.GetValues<Choice>()
            .Select(c => new ChoiceDto((int)c, c.ToString()));

        return Task.FromResult(choices);
    }
}