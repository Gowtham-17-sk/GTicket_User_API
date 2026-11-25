using GTicket_User_API.Models.Email;
using GTicket_User_API.Services.Interface;
using GTicket_User_API.Settings;
using Microsoft.Extensions.Options;

namespace GTicket_User_API.Services;

public class EmailDispatchService(
    IEmailContentHelper contentHelper,
    IEmailTemplateService templateService,
    IEmailService emailService,
    IEmailLogStore logStore,
    IOptions<EmailSettings> options,
    ILogger<EmailDispatchService> logger) : IEmailDispatchService
{
    public async Task<SendMailResponseModel> SendAsync(SendMailRequestModel request, CancellationToken cancellationToken)
    {
        // 1. Helper: mail type paathu template, subject, values ready (thappu na 400)
        var content = contentHelper.emailContent(request);

        // 2. Template service: HTML ready
        var html = await templateService.RenderAsync(content.TemplateFile, content.Values, cancellationToken);

        // 3. Mail object
        var message = new EmailMessageModel
        {
            ToName = request.ToName,
            ToEmail = request.ToMail,
            Subject = content.Subject,
            HtmlBody = html,
            TextBody = content.TextBody
        };

        var log = new EmailLogModel
        {
            LogId = Guid.NewGuid(),
            MailType = request.MailType,
            FromEmail = options.Value.FromEmail,   // settings la irundhu, request la illa
            ToEmail = request.ToMail,
            Subject = content.Subject
        };

        // 4. Send + 5. Log (success / failed rendukkum)
        try
        {
            await emailService.SendAsync(message, CancellationToken.None);
            log.Status = "Sent";
        }
        catch (Exception ex)
        {
            log.Status = "Failed";
            log.ErrorMessage = ex.GetType().Name;
            log.SentAtUtc = DateTime.UtcNow;
            await SaveLogSafeAsync(log);
            throw;
        }

        log.SentAtUtc = DateTime.UtcNow;
        await SaveLogSafeAsync(log);

        return new SendMailResponseModel
        {
            Message = "Mail sent successfully",
            StatusCode = 200,
            LogId = log.LogId
        };
    }

    public Task<IReadOnlyList<EmailLogModel>> GetLogsAsync(CancellationToken cancellationToken)
        => logStore.GetAllAsync(cancellationToken);

    // Log save fail aanaalum mail already pogirukku, so exception ah mela anuppaadhu
    private async Task SaveLogSafeAsync(EmailLogModel log)
    {
        try
        {
            await logStore.AddAsync(log, CancellationToken.None);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not save email log {LogId}", log.LogId);
        }
    }
}