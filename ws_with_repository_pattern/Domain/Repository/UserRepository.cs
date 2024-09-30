using Microsoft.EntityFrameworkCore;
using ws_with_repository_pattern.Application.Dto.Auth;
using ws_with_repository_pattern.Domain.Contract;
using ws_with_repository_pattern.Domain.DbContext;
using ws_with_repository_pattern.Domain.Entity;

namespace ws_with_repository_pattern.Domain.Repository;

public class UserRepository: IUserRepository
{
    private readonly KazutoDbContext _userDbContext;

    public UserRepository(KazutoDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }
    
    public async Task InsertUser(User user, UserRegistrationRequestDto request)
    {
        // Start a new database transaction
        using var transaction = await _userDbContext.Database.BeginTransactionAsync();
    
        try
        {
            // Add the user to the database
            await _userDbContext.AddAsync(user);
        
            // Create a new UserRoleMapping and add it to the database
            var role = new UserRoleMapping
            {
                userId = user.id,
                roleId = Guid.Parse(request.role_id),
            };
            await _userDbContext.Set<UserRoleMapping>().AddAsync(role);
            
            // add access
             await _userDbContext.Set<UserAccessMapping>().AddAsync(new UserAccessMapping
            {
                user_id = user.id,
                role_id = Guid.Parse(request.role_id),
                read = request.read,
                delete = request.delete,
                write = request.write,
            });
        
            // Save changes to the database
            await _userDbContext.SaveChangesAsync();
        
            // Commit the transaction
            await transaction.CommitAsync();
        }
        catch
        {
            // Roll back the transaction if any error occurs
            await transaction.RollbackAsync();
            throw; // Optionally rethrow the exception for further handling
        }
    }

    public Task InsertUserRoleMapping(string userId, Guid roleId)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> GetUser(string email)
    {
        return await _userDbContext.Set<User>().FirstOrDefaultAsync(x => x.email == email);
    }

    public async Task<List<UserRoleMapping>> GetUserRoles(string email)
    {
        var user = await GetUser(email);

        if (user != null)
        {
            return await _userDbContext.Set<UserRoleMapping>()
                .AsNoTracking()
                .Where(x => x.userId == user.id).ToListAsync();
        }
        
        return new List<UserRoleMapping>();
    }

    public async Task<List<MasterRole>> GetMasterRoles()
    {
        return await _userDbContext.Set<MasterRole>().Where(x => x.deleted_at == null).ToListAsync();
    }

    public async Task<UserAccessMapping?> GetUserAccess(string userId, Guid roleId)
    {
        var access = await _userDbContext.Set<UserAccessMapping>()
            .FirstOrDefaultAsync(x => x.user_id == userId &&
                                      x.role_id == roleId);
        return access;
    }
    
    public async Task<UserAccessMapping?> GetUserAccessMapping(string userId, Guid roleId)
    {
        var userAccess = await _userDbContext.Set<UserAccessMapping>().FirstOrDefaultAsync(x => x.user_id == userId &&
            x.role_id == roleId);

        return userAccess;
    }

    public async Task UpdateUserAccess(UserAccessMapping accessMapping)
    {
        _userDbContext.Set<UserAccessMapping>().Update(accessMapping);
        await _userDbContext.SaveChangesAsync();
    }
}