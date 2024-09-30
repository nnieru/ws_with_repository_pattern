using ws_with_repository_pattern.Application.Dto.Auth;
using ws_with_repository_pattern.Domain.Entity;

namespace ws_with_repository_pattern.Domain.Contract;

public interface IUserRepository
{
    public Task InsertUser(User user, UserRegistrationRequestDto request);
    public Task InsertUserRoleMapping(string userId, Guid roleId);
    public Task<User?> GetUser(string email);
    public Task<List<UserRoleMapping>> GetUserRoles (string email);
    public Task<List<MasterRole>> GetMasterRoles();
    public Task<UserAccessMapping?> GetUserAccess(string userId, Guid roleId);
    public Task UpdateUserAccess(UserAccessMapping accessMapping);
}