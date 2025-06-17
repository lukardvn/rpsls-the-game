using FluentValidation;
using MediatR;
using rpsls.Application.Common.Exceptions;
using rpsls.Application.Common.Messaging;
using ValidationException = rpsls.Application.Common.Exceptions.ValidationException;

namespace rpsls.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : ICommand<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken ct)
    {
        var context = new ValidationContext<TRequest>(request);

        var errors = _validators.Select(val => val.Validate(context))
            .Where(valResult => !valResult.IsValid)
            .SelectMany(valResult => valResult.Errors)
            .Select(valError => new ValidationError(valError.PropertyName, valError.ErrorMessage))
            .ToList();

        if (errors.Any())
            throw new ValidationException(errors);
        
        var response = await next(ct);

        return response;
    }
}