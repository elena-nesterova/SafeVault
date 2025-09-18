using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

using SafeVault.Services;

namespace SafeVault.Pages;

[Authorize(Roles = "Admin")]
public class AdminOnlyPageModel : PageModel
{
    private readonly ILogger<AdminOnlyPageModel> _logger;
    private readonly UserService _userService;

    public List<string?>? Users { get; set; }

    public AdminOnlyPageModel(ILogger<AdminOnlyPageModel> logger, UserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    public async Task OnGetAsync()
    {
        try
        {
            Users = await _userService.GetAllRegisteredUsersNamesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users.");
            Users = null;
        }
    }
    
}