using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;  // ← для [FromBody]
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using BookManagement.Data;
using BookManagement.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMudServices();
builder.Services.AddHttpClient(); // ← для HttpClient в компонентах

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    

    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/access-denied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.Cookie.IsEssential = true;
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

// ← Эндпоинты ДО MapRazorComponents
app.MapPost("/api/login", async (
    SignInManager<ApplicationUser> signInManager,
    [FromBody] LoginRequest request) =>
{
    var result = await signInManager.PasswordSignInAsync(
        request.Email, 
        request.Password, 
        isPersistent: false, 
        lockoutOnFailure: false);

    if (result.Succeeded)
        return Results.Ok(new { success = true, redirectUrl = "/profile" });
    else if (result.IsLockedOut)
        return Results.BadRequest(new { error = "Аккаунт заблокирован" });
    else if (result.IsNotAllowed)
        return Results.BadRequest(new { error = "Вход не разрешён" });
    else
        return Results.BadRequest(new { error = "Неверный email или пароль" });
});

app.MapRazorComponents<BookManagement.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();

// ← Records для запросов (в конце файла)
public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password, string Username);