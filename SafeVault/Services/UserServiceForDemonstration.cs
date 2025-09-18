using SafeVault.Models;
using SafeVault.Data;
using SafeVault.Security;
using Microsoft.EntityFrameworkCore;

namespace SafeVault.Services;

// This service is for demonstration purposes only and does NOT use ASP.NET Core Identity
public class UserServiceForDemonstration
{
    private const string AllowedSpecialCharacters = "!@#$%^&*?.";
    private readonly SafeVaultDbContext _context_for_demonstration;

    public UserServiceForDemonstration(SafeVaultDbContext context)
    {
        _context_for_demonstration = context;
    }

    // Validate input to prevent SQL Injection and XSS
    private bool IsValidInput(string input)
    {
        return (ValidationHelpers.IsValidInput(input, AllowedSpecialCharacters) &&
        ValidationHelpers.IsValidXSSInput(input));
    }

    // For demonstration purposes only: validate user against the local user store without Identity
    // This method is NOT using ASP.NET Core Identity and is for demonstration purposes only
    public async Task<bool> LoginUserAsync_ForDemonstration(string username, string password)
    {
        if (!IsValidInput(username) || !IsValidInput(password))
        {
            return false;
        }

        // Find user by username
        var user = await _context_for_demonstration.SafeVaultUsersForDemonstration
            .FirstOrDefaultAsync(u => u.UserName_forDemonstration == username);

        if (user == null)
            return false;

        // Verify password using bcrypt
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash_forDemonstration);

        return isPasswordValid;

    }

    // Register a new user for demonstration purposes only
    // This method is NOT using ASP.NET Core Identity and is for demonstration purposes only
    public async Task<bool> RegisterUserAsync_ForDemonstration(string username, string email, string password)
    {
        if (!IsValidInput(username) || !IsValidInput(email) || !IsValidInput(password))
        {
            return false;
        }

        // Check if username already exists
        if (await _context_for_demonstration.SafeVaultUsersForDemonstration.AnyAsync(u => u.UserName_forDemonstration == username))
        {
            return false;
        }

        // Hash password using bcrypt
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            UserName = username,
            Email = email,
            UserName_forDemonstration = username,
            Email_forDemonstration = email,
            PasswordHash_forDemonstration = hashedPassword
        };

        _context_for_demonstration.SafeVaultUsersForDemonstration.Add(user);
        await _context_for_demonstration.SaveChangesAsync();
        return true;
    }

    // Get all registered users' names
    public async Task<List<string>?> GetAllRegisteredUsersNamesAsync()
    {
        if (_context_for_demonstration.SafeVaultUsersForDemonstration == null || !_context_for_demonstration.SafeVaultUsersForDemonstration.Any())
            return null;    
        
        return await _context_for_demonstration.SafeVaultUsersForDemonstration.Select(u => u.UserName_forDemonstration).ToListAsync();        
    }
    
}