namespace GTicket_User_API.Settings
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = "GTicket";
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
        public string InviteBaseUrl { get; set; } = string.Empty;
        public string InvitedBy { get; set; } = "GTicket Team";
        public int InviteExpiryDays { get; set; } = 7;
    }
}
