using System.Text.Json.Serialization;

namespace GTicket_User_API.Models
{
    public class SendInviteEmailRequestModel
    {
        [JsonPropertyName("recipientEmail")]
        public string RecipientEmail { get; set; } = string.Empty;

        [JsonPropertyName("recipientName")]
        public string RecipientName { get; set; } = string.Empty;

        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("inviteLink")]
        public string InviteLink { get; set; } = string.Empty;
    }
}
