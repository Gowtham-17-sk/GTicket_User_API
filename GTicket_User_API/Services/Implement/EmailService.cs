using GTicket_User_API.Services.Interface;
using GTicket_User_API.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace GTicket_User_API.Services.Implement
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<EmailSettings> emailSettings,
            IWebHostEnvironment environment,
            ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _environment = environment;
            _logger = logger;
        }

        public async Task SendInviteEmailAsync(string recipientEmail, string recipientName, string role, string inviteLink, CancellationToken cancellationToken)
        {
            ValidateSettings();

            var templatePath = Path.Combine(_environment.ContentRootPath, "EmailTemplates", "InviteMail.html");

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException("Invite email template file not found.", templatePath);
            }

            var htmlContent = await File.ReadAllTextAsync(templatePath, cancellationToken);
            var renderedHtml = ReplaceTemplateValues(htmlContent, recipientName, role, inviteLink);

            using var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = "You're invited to GTicket",
                Body = renderedHtml,
                IsBodyHtml = true
            };

            message.To.Add(recipientEmail);

            using var smtpClient = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
            {
                EnableSsl = _emailSettings.EnableSsl,
                Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password)
            };

            try
            {
                await smtpClient.SendMailAsync(message, cancellationToken);
                _logger.LogInformation("Invite email sent to {RecipientEmail}", recipientEmail);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(
                    ex,
                    "SMTP send failed. Host={SmtpHost}, Port={SmtpPort}, EnableSsl={EnableSsl}, UserName={UserName}, Sender={SenderEmail}, Recipient={RecipientEmail}, StatusCode={StatusCode}, Inner={InnerMessage}",
                    _emailSettings.SmtpHost,
                    _emailSettings.SmtpPort,
                    _emailSettings.EnableSsl,
                    _emailSettings.UserName,
                    _emailSettings.SenderEmail,
                    recipientEmail,
                    ex.StatusCode,
                    ex.InnerException?.Message);

                throw new SmtpException(
                    $"SMTP send failed for recipient '{recipientEmail}' using host '{_emailSettings.SmtpHost}:{_emailSettings.SmtpPort}'. See inner exception for details.",
                    ex);
            }
        }

        private string ReplaceTemplateValues(string template, string recipientName, string role, string inviteLink)
        {
            return template
                .Replace("{{InvitedBy}}", _emailSettings.InvitedBy)
                .Replace("{{Role}}", role)
                .Replace("{{RecipientName}}", recipientName)
                .Replace("{{InviteLink}}", inviteLink)
                .Replace("{{ExpiryDays}}", _emailSettings.InviteExpiryDays.ToString())
                .Replace("{{Year}}", DateTime.UtcNow.Year.ToString());
        }

        private void ValidateSettings()
        {
            if (string.IsNullOrWhiteSpace(_emailSettings.SmtpHost) ||
                string.IsNullOrWhiteSpace(_emailSettings.SenderEmail) ||
                string.IsNullOrWhiteSpace(_emailSettings.UserName) ||
                string.IsNullOrWhiteSpace(_emailSettings.Password))
            {
                throw new InvalidOperationException("EmailSettings are missing required SMTP configuration values.");
            }

            if (_emailSettings.SmtpPort <= 0 || _emailSettings.SmtpPort > 65535)
            {
                throw new InvalidOperationException("EmailSettings.SmtpPort must be between 1 and 65535.");
            }
        }
    }
}
