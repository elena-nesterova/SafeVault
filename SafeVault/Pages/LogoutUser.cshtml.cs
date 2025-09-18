using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using SafeVault.Services;

namespace SafeVault.Pages;

public class LogoutUserModel: PageModel
{
    private readonly ILogger<LoginUserModel> _logger;
    private readonly UserService _userService;

    public LogoutUserModel(ILogger<LoginUserModel> logger, UserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    
    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();
        
        await _userService.LogoutUserAsync();
        
        return RedirectToPage("/Index");
    }
}
