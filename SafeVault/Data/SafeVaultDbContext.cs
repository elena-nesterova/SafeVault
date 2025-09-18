using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using SafeVault.Models;

namespace SafeVault.Data;

public class SafeVaultDbContext : IdentityDbContext<User, IdentityRole, string>
{
    public SafeVaultDbContext(DbContextOptions<SafeVaultDbContext> options) : base(options)
    {          
    }

    public DbSet<SafeVault.Models.User> SafeVaultUsersForDemonstration { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<SafeVault.Models.User>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<SafeVault.Models.User>()
            .Property(u => u.UserName_forDemonstration)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<SafeVault.Models.User>()
            .Property(u => u.Email_forDemonstration)
            .IsRequired()
            .HasMaxLength(100);
    }
}