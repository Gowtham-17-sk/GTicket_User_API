using System.Text.Json.Serialization;

namespace GTicket_User_API.Models
{
    public class UserResponseModel
    {
        [JsonPropertyName("userID")]
        public long UserID { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("statusCode")]
        public int? StatusCode { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("token")]
        public string? Token { get; set; }

        [JsonPropertyName("expiresAtUTC")]
        public DateTime? ExpireAtUTC { get; set; }
    }
}
