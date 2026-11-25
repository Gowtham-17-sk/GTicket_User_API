using Newtonsoft.Json;

namespace GTicket_User_API.Settings
{
    public class EmailSettings
    {
        [JsonProperty("host")]
        public string Host { get; set; } = string.Empty;
        [JsonProperty("port")]
        public int Port { get; set; } = 587;
        [JsonProperty("fromEmail")]
        public string FromEmail { get; set; } = string.Empty;
        [JsonProperty("fromName")]
        public string FromName { get; set; } = "GTicket";
        [JsonProperty("username")]
        public string Username { get; set; } = string.Empty;
        [JsonProperty("password")]
        public string Password { get; set; } = string.Empty;
    }
}
