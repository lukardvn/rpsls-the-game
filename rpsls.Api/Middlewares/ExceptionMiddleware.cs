using Microsoft.AspNetCore.Mvc;
using rpsls.Application.Common.Exceptions;

namespace rpsls.Api.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred: {Message}", ex.Message);

            var exceptionDetails = GetExceptionDetails(ex);
            
            var problem = new ProblemDetails
            {
                Title = exceptionDetails.Title,
                Detail = exceptionDetails.Detail,
                Status = exceptionDetails.Status,
                Type = exceptionDetails.Type,
                Instance = context.Request.Path
            };

            if (exceptionDetails.Errors is not null)
                problem.Extensions["errors"] = exceptionDetails.Errors;
            
            context.Response.StatusCode = problem.Status.Value;

            await context.Response.WriteAsJsonAsync(problem);
        }
    }

    private static ExceptionDetails GetExceptionDetails(Exception exception)
    {
        return exception switch
        {
            ValidationException validationException => new ExceptionDetails(
                StatusCodes.Status400BadRequest,
                "ValidationFailure",
                "Validation error",
                "One or more validation errors occurred",
                validationException.Errors
                ),
            
            _ => new ExceptionDetails(
                StatusCodes.Status500InternalServerError, 
                "InternalServerError",
                "Internal Server Error", 
                "An unexpected error occurred", 
                null)
        };
    }

    private record ExceptionDetails(int Status, string Type, string Title, string Detail, IEnumerable<object>? Errors);
}