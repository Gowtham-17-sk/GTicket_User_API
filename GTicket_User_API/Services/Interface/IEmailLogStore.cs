using GTicket_User_API.Models.Email;

namespace GTicket_User_API.Services.Interface
{
    public interface IEmailLogStore
    {
        Task AddAsync(EmailLogModel log, CancellationToken cancellationToken);
        Task<IReadOnlyList<EmailLogModel>> GetAllAsync(CancellationToken cancellationToken);
    }
}
