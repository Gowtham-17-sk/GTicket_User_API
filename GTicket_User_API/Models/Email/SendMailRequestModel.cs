using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace GTicket_User_API.Models.Email
{
    public class SendMailRequestModel
    {
        [JsonProperty("mailType")]
        public MailType MailType { get; set; }
        [Required]
        [JsonProperty("toMail")]
        public string ToMail { get; set; }
        [Required]
        [JsonProperty("toName")]
        public string ToName { get; set; }
        public Dictionary<string, JsonElement> TemplateData { get; set; } = new ();
    }
}
