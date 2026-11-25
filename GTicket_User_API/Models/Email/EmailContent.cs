using Newtonsoft.Json;

namespace GTicket_User_API.Models.Email;

public class EmailContent
{
    [JsonProperty("templateFile")]
    public string TemplateFile { get; set; } = string.Empty;
    [JsonProperty("subject")]
    public string Subject { get; set; } = string.Empty;
    [JsonProperty("textBody")]
    public string TextBody { get; set; } = string.Empty;
    [JsonProperty("values")]
    public Dictionary<string, string> Values { get; set; } = new();
}