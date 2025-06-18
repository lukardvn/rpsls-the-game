using FluentValidation.TestHelper;
using rpsls.Application.Commands;

namespace rpsls.Application.Tests.Commands;

public class PlayCommandValidatorTests
{
    private readonly PlayCommandValidator _validator = new();

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Handle_WhenValidChoice_ShouldNotHaveValidationError(int validChoice)
    {
        var command = new PlayCommand(validChoice, null);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.Choice);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(999)]
    public void Handle_WhenInvalidChoice_ShouldHaveValidationError(int invalidChoice)
    {
        var command = new PlayCommand(invalidChoice, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Choice)
            .WithErrorMessage($"Invalid player choice: {invalidChoice}");
    }
}