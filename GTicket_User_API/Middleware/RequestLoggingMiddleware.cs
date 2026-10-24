using System.Diagnostics;

namespace GTicket_User_API.Middleware
{
    public class RequestLoggingMiddleware(RequestDelegate next,ILogger<RequestLoggingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            logger.LogInformation("Incoming request: {method} {url}", context.Request.Method, context.Request.Path);
            
            var sw = Stopwatch.StartNew();
            await next(context);
            sw.Stop();
            logger.LogInformation("Outgoing response: {statusCode}", context.Response.StatusCode);
        }
    }
}
