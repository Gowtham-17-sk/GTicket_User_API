using Newtonsoft.Json;

namespace GTicket_User_API.Models.Email
{
    public class SendMailResponseModel
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }
        [JsonProperty("logId")]
        public Guid LogId { get; set; }
    }
}
