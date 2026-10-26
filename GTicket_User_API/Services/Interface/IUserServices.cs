using GTicket_User_API.Models;

namespace GTicket_User_API.Services.Interface
{
    public interface IUserServices
    {
        public Task<UserResponseModel> CreateUserAsync(CreateUserRequestModel requestModel,CancellationToken cancellationToken);
        public Task<List<RoleResponse>> GetUserRoleAsync(CancellationToken cancellationToken);
    }
}
