using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AgileBoard.Adapters.WebApi.Filters;

public class ExceptionHandlingMiddleware : IExceptionHandler
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        var problemDetails = new ProblemDetails();

        if (exception is Application.UseCases.Sprints.SprintOverlapException overlapEx)
        {
            problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
            problemDetails.Title = "Bad Request";
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Detail = overlapEx.Message;
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        else if (exception is Application.UseCases.Sprints.NotFoundException notFoundEx)
        {
            problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
            problemDetails.Title = "Not Found";
            problemDetails.Status = StatusCodes.Status404NotFound;
            problemDetails.Detail = notFoundEx.Message;
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        }
        else
        {
            problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
            problemDetails.Title = "Internal Server Error";
            problemDetails.Status = StatusCodes.Status500InternalServerError;
            problemDetails.Detail = "An unexpected error occurred.";
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
