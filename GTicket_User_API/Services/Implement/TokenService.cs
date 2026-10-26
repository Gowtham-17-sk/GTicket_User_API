
using GTicket_User_API.Services.Interface;
using GTicket_User_API.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace GTicket_User_API.Services.Implement
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly JwtSettings _jwt;

        public TokenService(
            ILogger<TokenService> logger,
            IOptions<JwtSettings> jwtOptions)
        {
            _logger = logger;
            _jwt = jwtOptions.Value;
        }

        public Task<(string Token, DateTime ExpiresAtUtc)>
            GenerateTokenAsync(
                string loginName,
                CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var expiresAt = DateTime.UtcNow
                .AddMinutes(_jwt.ExpiryMinutes);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, loginName),
                new(JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwt.Key));

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt,
                Issuer = _jwt.Issuer,
                Audience = _jwt.Audience,
                SigningCredentials = new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256)
            };

            var token = new JsonWebTokenHandler()
                .CreateToken(descriptor);

            _logger.LogInformation(
                "JWT token generated for {LoginName}",
                loginName);

            return Task.FromResult((token, expiresAt));
        }
    }
}
