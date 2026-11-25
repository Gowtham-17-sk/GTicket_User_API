namespace GTicket_User_API.Services.Interface
{
    public interface IEmailTemplateService
    {
        Task<string> RenderAsync(
            string templateFileName,
            IDictionary<string, string> values,
            CancellationToken cancellationToken);
    }
}
