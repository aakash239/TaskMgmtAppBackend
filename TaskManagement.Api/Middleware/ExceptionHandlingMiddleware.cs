using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.Exceptions;

namespace TaskManagement.Api.Middleware;

// Catches exceptions from anywhere below it and converts them to clean JSON responses.
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{


   
    private readonly RequestDelegate _next = next;                       // the next step in the pipeline
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;


    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // run the rest of the pipeline 
        }
        catch (ConflictException ex)
        {
           
            await WriteProblem(context, StatusCodes.Status409Conflict, "Conflict", ex.Message);
        }

        catch(NotFoundException ex)
        {
            await WriteProblem(context, StatusCodes.Status404NotFound, "Not Found", ex.Message);
        }
        catch(ForbiddenException ex)
        {
             await WriteProblem(context, StatusCodes.Status403Forbidden, "Forbidden", ex.Message);
        }

        catch (ArgumentException ex)
        {
            await WriteProblem(context, StatusCodes.Status400BadRequest, "Bad Request", ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            await WriteProblem(context, StatusCodes.Status400BadRequest, "Not Found", ex.Message);
        }


        catch (Exception ex)
        {
            
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