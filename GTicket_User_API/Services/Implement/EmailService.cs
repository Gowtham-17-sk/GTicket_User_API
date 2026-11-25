using GTicket_User_API.Models.Email;
using GTicket_User_API.Services.Interface;
using GTicket_User_API.Settings;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace GTicket_User_API.Services.Implement;

public class EmailService(
    IOptions<EmailSettings> options,
    ILogger<EmailService> logger) : IEmailService
{
    private readonly EmailSettings _settings = options.Value;

    public async Task SendAsync(
        EmailMessageModel email,
        CancellationToken cancellationToken)
    {
        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(
                _settings.FromName,
                _settings.FromEmail));

        message.To.Add(
            new MailboxAddress(
                email.ToName,
                email.ToEmail));

        message.Subject = email.Subject;

        message.Body = new BodyBuilder
        {
            HtmlBody = email.HtmlBody,
            TextBody = email.TextBody
        }.ToMessageBody();

        using var client = new SmtpClient();

        string stage = "Connecting";

        try
        {
            client.Timeout = 60000;
            client.CheckCertificateRevocation = false;

            // Step 1: Connect to Gmail SMTP
            stage = "Connecting to SMTP server";

            await client.ConnectAsync(
            _settings.Host,
            _settings.Port,
            SecureSocketOptions.SslOnConnect,
            cancellationToken);

            logger.LogInformation("SMTP connection successful.");

            // Step 2: Authenticate
            stage = "SMTP Authentication";

            await client.AuthenticateAsync(
                _settings.Username,
                _settings.Password,
                cancellationToken);

            logger.LogInformation("SMTP authentication successful.");

            // Step 3: Send Email
            stage = "Sending Email";

            await client.SendAsync(
                message,
                cancellationToken);

            logger.LogInformation(
                "Email sent successfully to {Email}",
                email.ToEmail);

            // Step 4: Disconnect
            stage = "Disconnecting";

            await client.DisconnectAsync(
                true,
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Email failed at stage: {Stage}. Recipient: {Email}. Error: {Error}",
                stage,
                email.ToEmail,
                ex.Message);

            throw;
        }
    }
}