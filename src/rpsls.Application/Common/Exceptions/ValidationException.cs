namespace rpsls.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public IReadOnlyCollection<ValidationError> Errors { get; }

    public ValidationException(IReadOnlyCollection<ValidationError> errors) : base("Validation failed.")
    {
        Errors = errors;
    }
}

public record ValidationError(string PropertyName, string ErrorMessage);