using FluentValidation.TestHelper;
using rpsls.Application.Commands;

namespace rpsls.Application.Tests.Commands;

public class UserPlayCommandValidatorTests
{
    private readonly UserPlayCommandValidator _validator = new();

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Should_Not_Have_Validation_Error_For_Valid_Choice(int validChoice)
    {
        var command = new UserPlayCommand(validChoice, null);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.Choice);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(999)]
    public void Should_Have_Validation_Error_For_Invalid_Choice(int invalidChoice)
    {
        var command = new UserPlayCommand(invalidChoice, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Choice)
            .WithErrorMessage($"Invalid player choice: {invalidChoice}");
    }
}