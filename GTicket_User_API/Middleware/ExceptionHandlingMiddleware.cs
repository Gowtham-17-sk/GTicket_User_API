using GTicket_User_API.Common.Exceptions;
using System.Text.Json;

namespace GTicket_User_API.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
			try
			{
				await next(context);
			}
			catch (Exception ex)
			{
				var (statusCode, message) = ex switch
				{

					NotFoundException => (404, ex.Message),
					ConflictException => (409, ex.Message),
					_ => (500, "An unexpected error occurred.")

				};

                if (statusCode == StatusCodes.Status500InternalServerError)
                    logger.LogError(ex, "Unhandled exception");

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                var body = new ErrorResponse { StatusCode = statusCode, Message = message };
                await context.Response.WriteAsync(JsonSerializer.Serialize(body,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
            }

        }
    }
}
