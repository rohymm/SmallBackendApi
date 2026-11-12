using System.Diagnostics;

namespace SmallBackendApi.Middleware
{
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTimingMiddleware> _logger;

        // _next is the next middleware in the pipeline
        public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            // Pass the request to the next middleware
            await _next(context);

            stopwatch.Stop();

            // Structured log message (this also covers the Logging topic)
            _logger.LogInformation("Request {Method} {Path} finished with status {StatusCode} in {ElapsedMs} ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}