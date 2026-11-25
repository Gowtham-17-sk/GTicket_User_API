using GTicket_User_API.Common.Exceptions;
using GTicket_User_API.Models.Email;
using GTicket_User_API.Services.Interface;
using System.Text.Json;

namespace GTicket_User_API.Services.Implement
{
    public class EmailContentHelper : IEmailContentHelper
    {

        private sealed record EmailTypeDefinition(string TemplateFile, string[] RequiredFiles, Func<IDictionary<string, string>,string> Subject,
            Func<IDictionary<string, string>,string> TextBody);

        private static readonly Dictionary<MailType, EmailTypeDefinition> _emailTypeDefinitions = new()
        {
            [MailType.Invite] = new(
            "InviteEmail.html",
            ["Role", "InvitedBy", "InviteLink", "ExpiryDays"],
            _ => "Your GTicket invitation",
            v => $"{v["InvitedBy"]} has invited you to join GTicket as {v["Role"]}. " +
                 $"Accept your invitation: {v["InviteLink"]} (expires in {v["ExpiryDays"]} days)"),

            [MailType.Ticket] = new(
            "TicketEmail.html",
            ["BookingId", "MovieName", "TheatreName", "ShowDate", "ShowTime", "Seats", "TotalAmount"],
            v => $"Your ticket: {v["MovieName"]} ({v["BookingId"]})",
            v => $"Booking {v["BookingId"]} confirmed. {v["MovieName"]}, {v["TheatreName"]}, " +
                 $"{v["ShowDate"]} {v["ShowTime"]}, Seats: {v["Seats"]}.")
        };

        public EmailContent emailContent(SendMailRequestModel request)
        {
            if (!_emailTypeDefinitions.TryGetValue(request.MailType, out var definition))
                throw new BadRequestException($"Unsupported mail type: {request.MailType}");

            // 1. JSON data ah case-insensitive dictionary ah maathurom ("role" / "Role" rendum ok)
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var (key, element) in request.TemplateData)
            {
                if (element.ValueKind is JsonValueKind.String or JsonValueKind.Number)
                    data[key] = element.ToString();
            }

            // 2. Venda keys mattum eduthu, template la irukkura same name la values ready
            var values = new Dictionary<string, string>();
            var missing = new List<string>();

            foreach (var key in definition.RequiredFiles)
            {
                if (data.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
                    values[key] = value.Trim();
                else
                    missing.Add(key);
            }

            if (missing.Count > 0)
                throw new BadRequestException($"Missing data for {request.MailType}: {string.Join(", ", missing)}");

            // 3. Ella type kum common values
            values["RecipientName"] = request.ToName.Trim();
            values["Year"] = DateTime.UtcNow.Year.ToString();

            // 4. Type specific validation
            if (request.MailType == MailType.Invite)
                ValidateInvite(values);

            return new EmailContent
            {
                TemplateFile = definition.TemplateFile,
                Subject = definition.Subject(values),
                TextBody = definition.TextBody(values),
                Values = values
            };
        }

        private static void ValidateInvite(Dictionary<string, string> values)
        {
            if (!Uri.TryCreate(values["InviteLink"], UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp))
                throw new BadRequestException("InviteLink must be a valid http/https link.");

            if (!int.TryParse(values["ExpiryDays"], out var days) || days is < 1 or > 30)
                throw new BadRequestException("ExpiryDays must be a number between 1 and 30.");

        }
    }
}
