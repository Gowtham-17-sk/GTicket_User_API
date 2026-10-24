using System.Text.Json.Serialization;

namespace GTicket_User_API.Models
{
    public class RoleResponse
    {
        [JsonPropertyName("roleID")]
        public long RoleID { get; set; }

        [JsonPropertyName("roleName")]
        public string RoleName { get; set; } = string.Empty;
    }
}
