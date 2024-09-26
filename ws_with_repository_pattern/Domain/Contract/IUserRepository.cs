using ws_with_repository_pattern.Domain.Entity;

namespace ws_with_repository_pattern.Domain.Contract;

public interface IUserRepository
{
    public Task InsertUser(User user);
    public Task InsertUserRoleMapping(string userId, Guid roleId);
    public Task<User?> GetUser(string email);
}