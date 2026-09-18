using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            DbUpdateException => (StatusCodes.Status500InternalServerError, "Database update failed"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(ExtractProblemDetails(httpContext, exception, statusCode, title));
    }

    private static ProblemDetailsContext ExtractProblemDetails(
        HttpContext httpContext,
        Exception exception,
        int statusCode,
        string title) => new()
    {
        HttpContext = httpContext,
        Exception = exception,
        ProblemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = httpContext.RequestServices
                            .GetRequiredService<IHostEnvironment>().IsDevelopment()
                                ? exception.Message
                                : null
        }
    };
}
