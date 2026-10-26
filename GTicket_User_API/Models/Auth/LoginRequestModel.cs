using System.Text.Json.Serialization;

namespace GTicket_User_API.Models.Auth
{
    public class LoginRequestModel
    {
        [JsonPropertyName("loginName")]
        public string LoginName { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
