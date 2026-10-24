using GTicket_User_API.Data;
using System.Security.Cryptography.Pkcs;

namespace GTicket_User_API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            return services;
        }
    }
}
