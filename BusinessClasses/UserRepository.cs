using SSRBusiness.BusinessFramework;
using SSRBusiness.Data;
using SSRBusiness.Entities;
using SSRBusiness.Support;
using Microsoft.EntityFrameworkCore;

namespace SSRBusiness.BusinessClasses;

/// <summary>
/// Business logic for User operations.
/// Converted from VB.NET UserEntity class.
/// </summary>
public class UserRepository : Repository<User>
{
    private readonly SsrDbContext _ssrContext;

    public UserRepository(SsrDbContext context) : base(context)
    {
        _ssrContext = context;
    }

    /// <summary>
    /// Loads an individual user by ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User entity or null</returns>
    public async Task<User?> LoadUserByUserIdAsync(int userId)
    {
        return await SingleOrDefaultAsync(u => u.UserId == userId);
    }

    /// <summary>
    /// Loads an individual user by user name (email)
    /// </summary>
    /// <param name="userName">User name</param>
    /// <returns>User entity or null</returns>
    public async Task<User?> LoadUserByUserNameAsync(string userName)
    {
        return await DbSet
            .Include(u => u.UserRoles)
            .SingleOrDefaultAsync(u => u.Email == userName || u.UserName == userName);
    }

    /// <summary>
    /// Authenticates a user password against stored hash
    /// </summary>
    /// <param name="user">User entity with password hash and salt</param>
    /// <param name="password">Plain text password to verify</param>
    /// <returns>True if authenticated, false otherwise</returns>
    public bool AuthenticateUserCredentials(User user, string password)
    {
        Console.WriteLine($"[AUTH DEBUG] Authenticating user: {user?.Email}");
        Console.WriteLine($"[AUTH DEBUG] User has password: {!string.IsNullOrEmpty(user?.Password)}");
        Console.WriteLine($"[AUTH DEBUG] User has salt: {!string.IsNullOrEmpty(user?.Salt)}");
        Console.WriteLine($"[AUTH DEBUG] User is active: {user?.IsActive}");
        
        if (user?.Password == null || user.Salt == null)
        {
            Console.WriteLine("[AUTH DEBUG] Authentication failed: Missing password or salt");
            return false;
        }

        var saltedHash = SaltedHash.Create(user.Salt, user.Password);
        var result = saltedHash.Verify(password);
        
        Console.WriteLine($"[AUTH DEBUG] Password verification result: {result}");
        
        return result;
    }

    /// <summary>
    /// Loads and authenticates a user based on username and password
    /// </summary>
    /// <param name="userName">User name (email)</param>
    /// <param name="password">Plain text password</param>
    /// <returns>Authenticated user or null</returns>
    public async Task<User?> AuthenticateAndLoadAsync(string userName, string password)
    {
        var user = await LoadUserByUserNameAsync(userName);
        
        if (user != null && AuthenticateUserCredentials(user, password))
        {
            return user;
        }

        return null;
    }

    /// <summary>
    /// Returns a full list of users ordered by email
    /// </summary>
    /// <returns>List of users</returns>
    public async Task<List<User>> GetUserListAsync()
    {
        return await DbSet
            .OrderBy(u => u.Email)
            .ToListAsync();
    }

    /// <summary>
    /// Returns a list of land managers (non-admin users) for lookup
    /// </summary>
    /// <returns>Lookup list of land managers</returns>
    public async Task<List<LookupListItem>> GetLandManListAsync()
    {
        return await DbSet
            .Where(u => u.Email!.ToUpper() != "SSRADMIN")
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Select(u => new LookupListItem
            {
                Value = u.UserId.ToString(),
                Description = $"{u.FirstName} {u.LastName}"
            })
            .ToListAsync();
    }

    /// <summary>
    /// Creates a new user with hashed password
    /// </summary>
    /// <param name="user">User entity</param>
    /// <param name="plainTextPassword">Plain text password to hash</param>
    /// <returns>Created user</returns>
    public async Task<User> CreateUserAsync(User user, string plainTextPassword)
    {
        var saltedHash = SaltedHash.Create(plainTextPassword);
        user.Password = saltedHash.Hash;
        user.Salt = saltedHash.Salt;
        user.IsActive = true;

        await AddAsync(user);
        await SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// Updates user password
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="newPassword">New plain text password</param>
    /// <returns>True if successful</returns>
    public async Task<bool> UpdatePasswordAsync(int userId, string newPassword)
    {
        var user = await LoadUserByUserIdAsync(userId);
        if (user == null)
            return false;

        var saltedHash = SaltedHash.Create(newPassword);
        user.Password = saltedHash.Hash;
        user.Salt = saltedHash.Salt;

        Update(user);
        await SaveChangesAsync();

        return true;
    }
}
