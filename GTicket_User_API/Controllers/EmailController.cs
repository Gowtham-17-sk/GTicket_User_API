using GTicket_User_API.Models.Email;
using GTicket_User_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GTicket_User_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController (IEmailDispatchService dispatchService) : ControllerBase
    {
        [HttpPost("send")]
        public async Task<IActionResult> Send(SendMailRequestModel request, CancellationToken cancellationToken)
        {
            var result = await dispatchService.SendAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet("logs")]
        public async Task<IActionResult> Logs(CancellationToken cancellationToken)
        {
            return Ok(await dispatchService.GetLogsAsync(cancellationToken));
        }
    }
}
