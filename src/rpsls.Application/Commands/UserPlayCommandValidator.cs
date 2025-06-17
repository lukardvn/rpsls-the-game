using FluentValidation;
using rpsls.Domain.Models;

namespace rpsls.Application.Commands;

public class UserPlayCommandValidator: AbstractValidator<UserPlayCommand>
{
    public UserPlayCommandValidator()
    {
        RuleFor(x => x.Choice)
            .Must(choice => Enum.IsDefined(typeof(Choice), choice))
            .WithMessage("Invalid player choice: {PropertyValue}");
    }
}