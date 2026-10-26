using GTicket_User_API.Models.Auth;

namespace GTicket_User_API.Services.Interface
{
    public interface IAuthRepository
    {
        Task<LoginUserModel>GetLoginUserAsync(string loginName, CancellationToken cancellationToken);
        Task<LoginResponseModel> LoginAsync(LoginRequestModel request, CancellationToken cancellationToken);

    }
}
