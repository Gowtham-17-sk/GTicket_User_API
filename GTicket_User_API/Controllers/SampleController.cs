using Dapper;
using GTicket_User_API.Common.Exceptions;
using GTicket_User_API.Data;
using GTicket_User_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GTicket_User_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController(IDbConnectionFactory connectionFactory) : ControllerBase
    {
        // 1. Simple ping: DB venda, API run aaguthaa nu paaka
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { message = "GTicket Identity API is running", time = DateTime.UtcNow });
        }

        // 2. DB check: SQL connect aaguthaa, Roles table la data irukkaa nu paaka
        [HttpGet("db-check")]
        public async Task<ActionResult<IEnumerable<RoleResponse>>> DbCheck()
        {
            using var connection = connectionFactory.CreateConnection();
            var roles = await connection.QueryAsync<RoleResponse>(
                "SELECT RoleId, RoleName FROM [dbo].[Role] ORDER BY RoleId");
            return Ok(roles);
        }

        // 3. Middleware test: 404 varanum
        [HttpGet("not-found-test")]
        public IActionResult NotFoundTest()
        {
            throw new NotFoundException("This is a test 404 from middleware.");
        }

        // 4. Middleware test: 500 varanum
        [HttpGet("error-test")]
        public IActionResult ErrorTest()
        {
            throw new Exception("Test crash");
        }
    }
}
