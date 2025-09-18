using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

using SafeVault.Services;
using SafeVault.Models;

namespace SafeVault.Pages;

public class LoginUserModel: PageModel
{
    private readonly ILogger<LoginUserModel> _logger;
    private readonly UserService _userService;

    public LoginUserModel(ILogger<LoginUserModel> logger, UserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [BindProperty]
    [Required]
    public string Username { get; set; } = "";

    [BindProperty]
    [Required]  
    public string Password { get; set; } = "";

    public string Message { get; set; } = "";

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        
        var result = await _userService.LoginUserAsync(Username, Password);
        if (result.Success)
        {
            Console.WriteLine($"!!!---User {Username} logged in successfully.");
            // Redirect or show success message
            return RedirectToPage("/Index");
        }
        else
        {
            Console.WriteLine($"!!!---Login failed for user {Username}: {result.Error}");
            Message = "Invalid login attempt.";
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }        
    }
}

