using GTicket_User_API.Data;
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

            return services;
        }
    }
}
