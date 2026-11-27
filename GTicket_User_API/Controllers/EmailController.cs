using GTicket_User_API.Models;
using GTicket_User_API.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace GTicket_User_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("sendInvite")]
        public async Task<IActionResult> SendInviteEmail([FromBody] SendInviteEmailRequestModel request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RecipientEmail) ||
                string.IsNullOrWhiteSpace(request.RecipientName) ||
                string.IsNullOrWhiteSpace(request.Role) ||
                string.IsNullOrWhiteSpace(request.InviteLink))
            {
                return BadRequest("recipientEmail, recipientName, role and inviteLink are required.");
            }

            await _emailService.SendInviteEmailAsync(
                request.RecipientEmail,
                request.RecipientName,
                request.Role,
                request.InviteLink,
                cancellationToken);

            return Ok(new { Message = "Invite email sent successfully" });
        }
    }
}
