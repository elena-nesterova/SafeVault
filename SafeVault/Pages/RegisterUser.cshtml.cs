using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

using SafeVault.Services;
using SafeVault.Models;

namespace SafeVault.Pages;

public class RegisterUserModel : PageModel
{
    private readonly ILogger<RegisterUserModel> _logger;
    private readonly UserService _userService;

    public RegisterUserModel(ILogger<RegisterUserModel> logger, UserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [BindProperty]
    [Required]
    public string Username { get; set; } = "";

    [BindProperty]
    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [BindProperty]
    [Required] 
    public string Password { get; set; } = "";

    [BindProperty]
    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = "";

    public string Message { get; set; } = "";

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostAsync()
    {        
        if (!ModelState.IsValid)
            return Page();
        
        var result = await _userService.RegisterUserAsync(Username, Email, Password);

        if (result.Succeeded)
        {
            Message = $"User {Username} registered successfully.";                 
        }
        else
        {
            Message = result.Errors.FirstOrDefault()?.Description ?? "Registration failed.";

            ModelState.AddModelError(string.Empty, Message);
        }
        return Page();
    }
}

