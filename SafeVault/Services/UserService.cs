using SafeVault.Models;
using SafeVault.Data;
using SafeVault.Security;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Identity;
using BCrypt.Net;

namespace SafeVault.Services;

public record UserServiceResult(bool Success, string? UserId = null, string? Error = null);

public class UserService
{
    private const string AllowedSpecialCharacters = "!@#$%^&*?.";    
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    // Validate input to prevent SQL Injection and XSS
    private bool IsValidInput(string input)
    {
        return (ValidationHelpers.IsValidInput(input, AllowedSpecialCharacters) &&
        ValidationHelpers.IsValidXSSInput(input));
    }
    
    // Validate user using Identity
    public async Task<UserServiceResult> LoginUserAsync(string username, string password, bool rememberMe = false)
    {
        if (!IsValidInput(username) || !IsValidInput(password))
            return new(Success: false, Error: "Invalid input");

        var user = await _userManager.FindByNameAsync(username);
        if (user is null)
        {
            await Task.Delay(50);
            return new(Success: false, Error: "Invalid credentials");
        }

        var check = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (!check.Succeeded) return new(Success: false, Error: "Invalid credentials");

        await _signInManager.SignInAsync(user, isPersistent: rememberMe);

        return new(Success: true, user.Id);
    }    

    // Register a new user
    public async Task<IdentityResult> RegisterUserAsync(string username, string email, string password)
    {
        if (!IsValidInput(username) || !IsValidInput(email) || !IsValidInput(password))
        {
            return IdentityResult.Failed(new IdentityError { Description = "Username and password are required." });
        }

        var user = await _userManager.FindByNameAsync(username);
        if (user is not null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Username already exists." });
        }

        user = new User
        {
            UserName = username,
            Email = email,
            UserName_forDemonstration = username,
            Email_forDemonstration = email,
            PasswordHash_forDemonstration = BCrypt.Net.BCrypt.HashPassword(password)
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return result;

        if (_userManager.Users.Count() == 1)
        {
            // First user - make admin
            await AssignRoleAsync(user, "Admin");
        }
        else
        {
            // Other users - regular user role
            await AssignRoleAsync(user, "User");
        }

        //await _signInManager.SignInAsync(user, isPersistent: false);

        return result;
    }
    
    // Get all registered users' names
    public async Task<List<string?>?> GetAllRegisteredUsersNamesAsync()
    {
        if (!_userManager.Users.Any())
            return null;
        
        return await _userManager.Users.Select(u => u.UserName).ToListAsync();        
    }

    // Assign a role to a user
    public async Task AssignRoleAsync(User user, string role)
    {
        // Create role if it doesn't exist
        if (!await _roleManager.RoleExistsAsync(role))
        {
            await _roleManager.CreateAsync(new IdentityRole(role));
        }
        await _userManager.AddToRoleAsync(user, role);
    }

    public async Task LogoutUserAsync()
    {
        await _signInManager.SignOutAsync();        
    }
}