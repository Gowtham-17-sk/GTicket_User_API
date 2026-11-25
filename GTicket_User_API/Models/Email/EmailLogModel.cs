using Newtonsoft.Json;

namespace GTicket_User_API.Models.Email;

public class EmailLogModel
{
    [JsonProperty("logId")]
    public Guid LogId { get; set; }
    [JsonProperty("mailType")]
    public MailType MailType { get; set; }
    [JsonProperty("fromEmail")]
    public string FromEmail { get; set; } = string.Empty;
    [JsonProperty("toEmail")]
    public string ToEmail { get; set; } = string.Empty;
    [JsonProperty("subject")]
    public string Subject { get; set; } = string.Empty;
    [JsonProperty("sentAtUtc")]
    public DateTime SentAtUtc { get; set; }
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;      // Sent / Failed
    [JsonProperty("errorMessage")]
    public string? ErrorMessage { get; set; }
}
