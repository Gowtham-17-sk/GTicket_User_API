namespace GTicket_User_API.Services.Interface
{
    public interface IEmailService
    {
        Task SendInviteEmailAsync(string recipientEmail, string recipientName, string role, string inviteLink, CancellationToken cancellationToken);
    }
}
