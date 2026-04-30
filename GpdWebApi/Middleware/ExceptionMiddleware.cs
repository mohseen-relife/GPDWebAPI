using System.Net;
using System.Text.Json;

namespace GpdWebApi.Middleware
{
    /// <summary>
    /// Global exception handler middleware.
    /// Catches unhandled exceptions and returns a consistent JSON error envelope
    /// instead of letting ASP.NET Core's default HTML error page leak.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next   = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error: {Message}", ex.Message);
                await WriteErrorAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await WriteErrorAsync(context, HttpStatusCode.InternalServerError,
                    "An unexpected error occurred. Please try again later.");
            }
        }

        private static async Task WriteErrorAsync(
            HttpContext context,
            HttpStatusCode statusCode,
            string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode  = (int)statusCode;

            var body = JsonSerializer.Serialize(new
            {
                success = false,
                message,
                totalRecords = 0,
                data = (object?)null
            });

            await context.Response.WriteAsync(body);
        }
    }
}
