using System.Data;

namespace GTicket_User_API.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();

        Task<IDbConnection>GTicketUserConnectionAsync(CancellationToken cancellationToken);
    }
}
