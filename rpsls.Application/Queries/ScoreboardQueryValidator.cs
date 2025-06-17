using FluentValidation;

namespace rpsls.Application.Queries;

//TODO: currently won't be triggered because of the custom Behavior that specifies Validation only to ICommand
public class ScoreboardQueryValidator : AbstractValidator<ScoreboardQuery>
{
    public ScoreboardQueryValidator()
    {
        RuleFor(x => x.Count)
            .GreaterThan(0)
            .WithMessage("Count must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Count cannot exceed 100");
    }
}