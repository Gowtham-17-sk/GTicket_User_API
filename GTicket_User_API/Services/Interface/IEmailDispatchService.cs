using GTicket_User_API.Models.Email;

namespace GTicket_User_API.Services.Interface
{
    public interface IEmailDispatchService
    {
        Task<SendMailResponseModel> SendAsync(SendMailRequestModel request, CancellationToken cancellationToken);
        Task<IReadOnlyList<EmailLogModel>> GetLogsAsync(CancellationToken cancellationToken);
    }
}
