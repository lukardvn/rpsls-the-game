using FluentValidation;
using MediatR;
using rpsls.Application.Common.Exceptions;
using rpsls.Application.Common.Messaging;
using ValidationException = rpsls.Application.Common.Exceptions.ValidationException;

namespace rpsls.Application.Common.Behaviors;

/// <summary>
/// Ensures that commands are validated before they run - and throws if something is invalid.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : ICommandBase
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken ct)
    {
        var context = new ValidationContext<TRequest>(request);

        var errors = validators.Select(val => val.Validate(context))
            .Where(valResult => !valResult.IsValid)
            .SelectMany(valResult => valResult.Errors)
            .Select(valError => new ValidationError(valError.PropertyName, valError.ErrorMessage))
            .ToList();

        if (errors.Count != 0)
            throw new ValidationException(errors);
        
        var response = await next(ct);

        return response;
    }
}