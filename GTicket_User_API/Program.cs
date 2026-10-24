using GTicket_User_API.Extensions;
using GTicket_User_API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ---------- 1. Services register (DI container) ----------
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddApplicationServices();     // DB connection factory

var app = builder.Build();

// ---------- 2. Middleware pipeline (order important) ----------
app.UseMiddleware<ExceptionHandlingMiddleware>();   // first: ella error um pidikkum
app.UseMiddleware<RequestLoggingMiddleware>();      // request time log

app.UseApiDocumentation(builder.Configuration);    // Swagger / Scalar

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();                               // controller URLs map

// ---------- 3. Start ----------
app.Run();
