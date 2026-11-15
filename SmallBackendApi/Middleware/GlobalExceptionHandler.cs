using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SmallBackendApi.Middleware
{
    // One central place that handles every unexpected exception
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // Log the full details on the server (Error level = unexpected failure)
            _logger.LogError(exception, "Unhandled exception on {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path);

            // Send only a generic message to the client
            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred",
                Detail = "Something went wrong on the server. Please try again later.",
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

            // true = the exception is handled, no need to run other handlers
            return true;
        }
    }
}