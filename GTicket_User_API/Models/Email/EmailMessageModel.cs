using Newtonsoft.Json;

namespace GTicket_User_API.Models.Email
{
    public class EmailMessageModel
    {
        [JsonProperty("toName")]
        public string ToName { get; set; } = string.Empty;
        [JsonProperty("toEmail")]
        public string ToEmail { get; set; } = string.Empty;
        [JsonProperty("subject")]           
        public string Subject { get; set; } = string.Empty;
        [JsonProperty("htmlBody")]
        public string HtmlBody { get; set; } = string.Empty;
        [JsonProperty("textBody")]
        public string TextBody { get; set; } = string.Empty;
    }
}
