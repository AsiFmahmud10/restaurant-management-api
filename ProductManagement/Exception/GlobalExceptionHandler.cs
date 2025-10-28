using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ProductManagement.Exception;

public class GlobalExceptionHandler : IExceptionHandler 
{
    private ProblemDetails problemDetails;
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, System.Exception exception, CancellationToken cancellationToken)
    {
        
        if (exception is ResourceNotFoundException or BadRequestException)
        {
            problemDetails = new ProblemDetails()
            {
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest,
                Type = "https://httpstatuses.com/400",
                Title = "Validation Error",   // how cancellation token works ?
                Instance = httpContext.Request.Path,
            };
         
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
        }
        
        if (exception is ValidationException)
        { 
            problemDetails = new ProblemDetails()
            {
                Detail = "InternalServerError",
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://httpstatuses.com/500",
                Title = "Server Error",
                Instance = httpContext.Request.Path,
            };
            
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
           
        }

        return true;
    }
}