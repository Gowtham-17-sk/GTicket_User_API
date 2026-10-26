using Dapper;
using GTicket_User_API.Data;
using GTicket_User_API.Models.Auth;
using GTicket_User_API.Services.Interface;
using Microsoft.AspNetCore.Identity;

namespace GTicket_User_API.Services.Implement
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ILogger<AuthRepository> _logger;
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ITokenService _tokenService;
        public AuthRepository(ILogger<AuthRepository> logger, IDbConnectionFactory connectionFactory, ITokenService tokenService)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
            _tokenService = tokenService;
        }

        public async Task<LoginUserModel> GetLoginUserAsync(string loginName, CancellationToken cancellationToken)
        {
            try
            {
                using var connection =await _connectionFactory.GTicketUserConnectionAsync(cancellationToken);

                const string query = @"
                    SELECT ud.UserID, ud.LoginName, ud.Password, r.RoleName
                    FROM dbo.UserDetails ud with(nolock)
                    INNER JOIN dbo.Role  r with(nolock) ON ud.RoleID = r.RoleID
                    WHERE ud.LoginName = @LoginName";

                var parameters = new { LoginName = loginName };
                var result = await connection.QueryFirstOrDefaultAsync<LoginUserModel>(query, parameters);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while fetching login user: {ex.Message}");
                throw ex;
            }
        }

        public async Task<LoginResponseModel> LoginAsync(LoginRequestModel request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await GetLoginUserAsync(request.LoginName, cancellationToken);
                // User illa, illa password thappu: rendukkum same message
                if (user is null)
                {
                    _logger.LogInformation("Login failed. Unknown user: {LoginName}", request.LoginName);
                    return new LoginResponseModel { Message = "Invalid login or password", StatusCode = 401 };
                }

                // Create la use pannina same type
                var hasher = new PasswordHasher<object>();
                var result = hasher.VerifyHashedPassword(null!, user.Password, request.Password);

                if (result == PasswordVerificationResult.Failed)
                {
                    _logger.LogInformation("Login failed. Wrong password for UserId: {UserId}", user.UserId);
                    return new LoginResponseModel { Message = "Invalid login or password", StatusCode = 401 };
                }

                var (token, expiresAt) = await _tokenService.GenerateTokenAsync(user.LoginName, cancellationToken);

                return new LoginResponseModel
                {
                    Message = "Login successful",
                    StatusCode = 200,
                    Token = token,
                    ExpiresAtUtc = expiresAt,
                    Role = user.RoleName
                };
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
