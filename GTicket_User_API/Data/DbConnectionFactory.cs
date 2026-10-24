using Microsoft.Data.SqlClient;
using System.Data;

namespace GTicket_User_API.Data
{
    public class DbConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
    {
        private readonly string _connectionString = configuration.GetConnectionString("GTicketUserConnection")
            ?? throw new InvalidOperationException("GTicketUserConnection is missing in appsettings.json");

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
