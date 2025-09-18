using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using SafeVault.Data;
using SafeVault.Models;

var builder = WebApplication.CreateBuilder(args);

var dataDir = Path.Combine(builder.Environment.ContentRootPath, "AppData");
Directory.CreateDirectory(dataDir);

var dbFile = Path.Combine(dataDir, builder.Configuration.GetConnectionString("DefaultConnection") ?? "safevault.db");
var connString = $"Data Source={dbFile};Cache=Shared;Foreign Keys=True";

// Configure in-memory database
builder.Services.AddDbContext<SafeVaultDbContext>(options =>
    options.UseSqlite(connString));
//options.UseInMemoryDatabase(builder.Configuration.GetConnectionString("InMemoryConnection") ?? "SafeVaultDb"));

// Register Identity services
builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        // For demonstration purposes, simplify password requirements
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 3;
        options.Password.RequiredUniqueChars = 1;
    })
    .AddEntityFrameworkStores<SafeVaultDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Register UserService as scoped
builder.Services.AddScoped<SafeVault.Services.UserService>();

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SafeVaultDbContext>();
    //db.Database.EnsureCreated();   
    await db.Database.MigrateAsync();  
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapRazorPages();

app.Run();
