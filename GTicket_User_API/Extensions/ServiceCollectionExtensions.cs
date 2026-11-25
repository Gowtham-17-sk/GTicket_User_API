using GTicket_User_API.Data;
using GTicket_User_API.Services;
using GTicket_User_API.Services.Implement;
using GTicket_User_API.Services.Interface;
using GTicket_User_API.Settings;
using System.Security.Cryptography.Pkcs;

namespace GTicket_User_API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration _configuration)
        {
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            services.AddTransient<IUserServices, UserServices>();
            services.Configure<JwtSettings>(_configuration.GetSection("Jwt"));
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<ITokenService, TokenService>();

            // Email services
            services.Configure<EmailSettings>(_configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton<IEmailContentHelper, EmailContentHelper>();
            services.AddSingleton<IEmailLogStore, JsonFileEmailLogStore>();
            services.AddScoped<IEmailDispatchService, EmailDispatchService>();

            return services;
        }
    }
}
