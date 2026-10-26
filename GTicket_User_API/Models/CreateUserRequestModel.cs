using System.Text.Json.Serialization;

namespace GTicket_User_API.Models
{
    public class CreateUserRequestModel
    {

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("loginName")]
        public string LoginName { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

    public class UpdateUserRequestModel
    {
        [JsonPropertyName("userID")]
        public long UserID { get; set; }

        [JsonPropertyName("UserDetails")]
        public UserDetailsJson UserDetails { get; set; }

    }
    public class UserDetailsJson
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("zipCode")]
        public string ZipCode { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }
    }
}
