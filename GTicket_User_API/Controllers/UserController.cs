using GTicket_User_API.Models;
using GTicket_User_API.Models.Auth;
using GTicket_User_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GTicket_User_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IAuthRepository _authService;

        public UserController(IUserServices userServices, IAuthRepository authService)
        {
            _userServices = userServices;
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _authService.LoginAsync(request, cancellationToken);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("createUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestModel requestModel)
        {
            var user = await _userServices.CreateUserAsync(requestModel, CancellationToken.None);
            return Ok(user);
        }
        // 2. DB check: SQL connect aaguthaa, Roles table la data irukkaa nu paaka
        [Authorize]
        [HttpGet("Roles")]
        public async Task<IActionResult> DbCheck()
        {
            var roles = await _userServices.GetUserRoleAsync(CancellationToken.None);
            return Ok(roles);
        }
    }
}
