using FluentValidation;
using rpsls.Domain.Models;

namespace rpsls.Application.Commands;

public class PlayCommandValidator : AbstractValidator<PlayCommand>
{
    public PlayCommandValidator()
    {
        RuleFor(x => x.Player)
            .Must(choice => Enum.IsDefined(typeof(Choice), choice))
            .WithMessage("Invalid player choice: {PropertyValue}");
    }
}