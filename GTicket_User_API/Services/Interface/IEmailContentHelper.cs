using GTicket_User_API.Models.Email;

namespace GTicket_User_API.Services.Interface
{
    public interface IEmailContentHelper
    {
        EmailContent emailContent(SendMailRequestModel request);
    }
}
