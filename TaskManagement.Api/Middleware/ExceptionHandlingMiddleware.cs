using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.Exceptions;

namespace TaskManagement.Api.Middleware;

// Catches exceptions from anywhere below it and converts them to clean JSON responses.
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;                       // the next step in the pipeline
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // run the rest of the pipeline (controller etc.)
        }
        catch (ConflictException ex)
        {
            // Expected business error -> 409 Conflict, no scary logging.
            await WriteProblem(context, StatusCodes.Status409Conflict, "Conflict", ex.Message);
        }
        catch (Exception ex)
        {
            // Unexpected -> log full details server-side, but hide them from the client.
            _logger.LogError(ex, "Unhandled exception");
            await WriteProblem(context, StatusCodes.Status500InternalServerError,
                "Server error", "An unexpected error occurred.");
        }
    }

    private static async Task WriteProblem(HttpContext context, int status, string title, string detail)
    {
        context.Response.StatusCode = status;                  // set HTTP status
        // ProblemDetails is the standard RFC 7807 error format.
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail
        });
    }
}