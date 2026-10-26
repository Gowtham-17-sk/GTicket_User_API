using Dapper;
using GTicket_User_API.Data;
using GTicket_User_API.Models;
using GTicket_User_API.Models.Auth;
using GTicket_User_API.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace GTicket_User_API.Services.Implement
{
    public class UserServices : IUserServices
    {
        private readonly ILogger<UserServices> _logger;
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IAuthRepository _authRepository;
        public UserServices(ILogger<UserServices> logger, IDbConnectionFactory connectionFactory, IAuthRepository authRepository)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
            _authRepository = authRepository;
        }

        public async Task<UserResponseModel> CreateUserAsync(CreateUserRequestModel requestModel, CancellationToken cancellationToken)
        {
            try
            {

                var roleTask = GetUserRoleAsync(requestModel.Role, cancellationToken);
                var emailTask = IsEmailExistsAsync(requestModel.Email, cancellationToken);

                await Task.WhenAll(roleTask, emailTask);

                var userRoleId = await roleTask;
                var emailExists = await emailTask;

                if (userRoleId == 0)
                {
                    _logger.LogInformation("Invalid Role: {Role}", requestModel.Role);

                    return new UserResponseModel
                    {
                        Message = $"Invalid Role: {requestModel.Role}",
                        StatusCode = 400
                    };
                }

                if (emailExists)
                {
                    return new UserResponseModel
                    {
                        Message = $"{requestModel.Email} already exists",
                        StatusCode = 409
                    };
                }

                using var connection = await _connectionFactory.GTicketUserConnectionAsync(cancellationToken);

                // password hashing logic can be added here before storing the password in the database
                var passwordHash = new PasswordHasher<Object>(); // Placeholder for hashed password
                string hashedPassword = passwordHash.HashPassword(
                    null!,
                    requestModel.Password
                );

                const string createUserQuery = @"
                    INSERT INTO dbo.UserDetails (RoleID,Email,LoginName,[Password],IsActive,CreatedBy,CreatedDate)
                    OUTPUT INSERTED.UserId
                    VALUES (@RoleID,@Email,@LoginName,@Password,@IsActive,@CreatedBy,@CreatedDate)
                ";

                var parameters = new
                {
                    RoleID = userRoleId,
                    Email = requestModel.Email,
                    LoginName = requestModel.LoginName,
                    Password = hashedPassword,
                    IsActive = true,
                    CreatedBy =  "System", // This can be replaced with the actual user creating the account
                    CreatedDate = DateTime.UtcNow
                };

                var userId = await connection.ExecuteScalarAsync<int>(createUserQuery, parameters);

                if (userId == 0)
                {
                    _logger.LogError("Failed to create user: {Email}", requestModel.Email);

                    return new UserResponseModel
                    {
                        Message = "Failed to create user",
                        StatusCode = 500
                    };
                }

                var tokenResult = await _authRepository.LoginAsync(new LoginRequestModel
                {
                    LoginName = requestModel.LoginName,
                    Password = requestModel.Password
                }, cancellationToken);


                return new UserResponseModel
                {
                    UserID = userId,
                    Message = "User created successfully",
                    StatusCode = 200,
                    Token = tokenResult.Token,
                    ExpireAtUTC = tokenResult.ExpiresAtUtc,
                    Role = requestModel.Role
                };
            }
            catch (Exception ex)
            {
                var InputParams = new
                {
                    request = JsonConvert.SerializeObject(requestModel)
                };
                _logger.LogInformation($"Exception in UserServices:CreateUserAsync Input: {JsonConvert.SerializeObject(InputParams)} Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<List<RoleResponse>> GetUserRoleAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var connection = await _connectionFactory.GTicketUserConnectionAsync(cancellationToken);
                const string query = @"
                    SELECT RoleId, RoleName
                    FROM dbo.Role WITH (NOLOCK)
                    ORDER BY RoleId";
                var roles = await connection.QueryAsync<RoleResponse>(
                new CommandDefinition(
                    query,
                    cancellationToken: cancellationToken));

                return roles.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user roles.");  
                throw;
            }
        }

        private async Task<int> GetUserRoleAsync(
            string role,
            CancellationToken cancellationToken)
        {
            using var connection =
                await _connectionFactory.GTicketUserConnectionAsync(cancellationToken);

            const string query = @"
                                    SELECT RoleId
                                    FROM dbo.Role WITH (NOLOCK)
                                    WHERE RoleName = @RoleName";

            return await connection.QuerySingleOrDefaultAsync<int>(
                query,
                new { RoleName = role });
        }

        private async Task<bool> IsEmailExistsAsync(
            string email,
            CancellationToken cancellationToken)
        {
            using var connection =
                await _connectionFactory.GTicketUserConnectionAsync(cancellationToken);

            const string query = @"
                                    SELECT UserId
                                    FROM dbo.UserDetails WITH (NOLOCK)
                                    WHERE Email = @Email";

            var userId = await connection.QuerySingleOrDefaultAsync<int>(
                query,
                new { Email = email });

            return userId != 0;
        }

    }
}
