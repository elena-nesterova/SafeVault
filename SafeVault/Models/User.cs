using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SafeVault.Models;

public class User : IdentityUser
{    
    [Required]
    public string UserName_forDemonstration { get; set; } = string.Empty;
    
    public string Email_forDemonstration { get; set; } = string.Empty;

    [Required]
    public string PasswordHash_forDemonstration { get; set; } = string.Empty;
}
