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
    
    public async Task InsertUser(User user)
    {
        // Start a new database transaction
        using var transaction = await _userDbContext.Database.BeginTransactionAsync();
    
        try
        {
            // Add the user to the database
            await _userDbContext.AddAsync(user);
        
            // Create a new UserRoleMapping and add it to the database
            await _userDbContext.Set<UserRoleMapping>().AddAsync(new UserRoleMapping
            {
                userId = user.id,
                roleId = Guid.Parse("205F4F22-8AFC-49FA-BC3F-1AE5589AEBBE")
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

    public Task<User?> GetUser(string email)
    {
        throw new NotImplementedException();
    }
}