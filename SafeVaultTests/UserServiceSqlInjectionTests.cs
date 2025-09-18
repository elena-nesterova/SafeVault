using Microsoft.EntityFrameworkCore;
using SafeVault.Data;
using SafeVault.Models;
using SafeVault.Services;

public class UserServiceSqlInjectionTests
{
    private SafeVaultDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<SafeVaultDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        return new SafeVaultDbContext(options);
    }

    [Fact]
    public async Task LoginUserAsync_ShouldNotAuthenticate_WithSqlInjectionInput()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        
        dbContext.SafeVaultUsersForDemonstration.Add(new User{
            UserName_forDemonstration = "testuser",
            PasswordHash_forDemonstration = "password123",
            Email_forDemonstration = "test@example.com" });
        dbContext.SaveChanges();

        var userService = new UserServiceForDemonstration(dbContext);

        // Simulate SQL injection attempt in username and password
        string maliciousUsername = "testuser' OR '1'='1";
        string maliciousPassword = "anything' OR 'x'='x";

        // Act
        var result = await userService.LoginUserAsync_ForDemonstration(maliciousUsername, maliciousPassword);

        // Assert
        Assert.False(result, "SQL Injection attempt should not authenticate the user.");
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldNotAllowSqlInjectionInput()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var userService = new UserServiceForDemonstration(dbContext);

        string maliciousUsername = "admin'; DROP TABLE Users; --";
        string email = "malicious@example.com";
        string password = "password123";

        //Act
        var result = await userService.RegisterUserAsync_ForDemonstration(maliciousUsername, email, password);

        // Assert
        Assert.False(result, "SQL Injection attempt should not register the user.");        
    }
}