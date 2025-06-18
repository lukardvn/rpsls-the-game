using FluentValidation;

namespace rpsls.Application.Queries;

/// <summary>
/// Currently, this validator won't be triggered because of the custom behavior that specifies validation only to ICommand.
///
/// If we wanted to trigger this one as well
/// 1. Remove the ICommand constraint from the behavior
/// or
/// 2. Introduce separate ValidationBehaviour for queries 
/// </summary>
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