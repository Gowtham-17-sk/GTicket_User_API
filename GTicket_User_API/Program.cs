using GTicket_User_API.Extensions;
using GTicket_User_API.Middleware;
using GTicket_User_API.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------- 1. Services register (DI container) ----------
builder.Services.AddControllers();
builder.Services.AddOpenApi(option =>
{
    option.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});
builder.Services.AddApplicationServices(builder.Configuration);   

var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
          ?? throw new InvalidOperationException("Jwt section missing");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name
        };
    });

builder.Services.AddAuthorization();






var app = builder.Build();

// ---------- 2. Middleware pipeline (order important) ----------
app.UseMiddleware<ExceptionHandlingMiddleware>();   // first: ella error um pidikkum
app.UseMiddleware<RequestLoggingMiddleware>();      // request time log

app.UseApiDocumentation(builder.Configuration);    // Swagger / Scalar

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();                               // controller URLs map

// ---------- 3. Start ----------
app.Run();
