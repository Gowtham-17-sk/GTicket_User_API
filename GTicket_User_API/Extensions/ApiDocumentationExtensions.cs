using Scalar.AspNetCore;

namespace GTicket_User_API.Extensions
{
    public static class ApiDocumentationExtensions
    {
        public static WebApplication UseApiDocumentation(this WebApplication app, IConfiguration config)
        {
            bool isSwagger = config.GetValue<bool>("IsSwagger");

            app.MapOpenApi();   // API oda JSON description: /openapi/v1.json

            if (isSwagger)
            {
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "GTicket Identity API v1");
                    options.RoutePrefix = "swagger";
                });
            }
            else
            {
                app.MapScalarApiReference(options => options.WithTitle("GTicket Identity API"));
            }

            // Root URL ku vandha correct UI ku redirect
            app.MapGet("/", () => Results.Redirect(isSwagger ? "/swagger" : "/scalar"))
               .ExcludeFromDescription();

            return app;
        }
    }
}
