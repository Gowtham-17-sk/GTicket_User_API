using Microsoft.Data.SqlClient;
using System.Data;

namespace GTicket_User_API.Data
{
    public class DbConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
    {
        private readonly string _connectionString = configuration.GetConnectionString("GTicketUserConnection")
            ?? throw new InvalidOperationException("GTicketUserConnection is missing in appsettings.json");

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IDbConnection> GTicketUserConnectionAsync(CancellationToken cancellationToken)
        {
            try
            {
                var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);
                return connection;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
