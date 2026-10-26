namespace GTicket_User_API.Models.Auth
{
    public class LoginResponseModel
    {
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpiresAtUtc { get; set; }
        public string? Role { get; set; }
    }
    public class LoginUserModel
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string LoginName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
