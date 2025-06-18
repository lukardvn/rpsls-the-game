using FluentValidation;
using rpsls.Domain.Models;

namespace rpsls.Application.Commands;

// Gets triggered by ExceptionMiddleware when this specific command is activated. If it fails, request is passed to ValidationBehavior.
public class PlayCommandValidator: AbstractValidator<PlayCommand>
{
    public PlayCommandValidator()
    {
        RuleFor(x => x.Choice)
            .Must(choice => Enum.IsDefined(typeof(Choice), choice))
            .WithMessage("Invalid player choice: {PropertyValue}");
    }
}