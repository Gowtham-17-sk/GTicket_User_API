using GTicket_User_API.Models.Email;

namespace GTicket_User_API.Services.Interface
{
    public interface IEmailService
    {
        Task SendAsync(EmailMessageModel email, CancellationToken cancellationToken);
    }
}
