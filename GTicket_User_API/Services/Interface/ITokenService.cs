using Newtonsoft.Json.Linq;

namespace GTicket_User_API.Services.Interface
{
    public interface ITokenService
    {
        Task<(string Token, DateTime ExpiresAtUtc)> GenerateTokenAsync(string loginName, CancellationToken cancellationToken);
    }
}
