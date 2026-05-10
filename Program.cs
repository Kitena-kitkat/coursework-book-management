using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using BookManagement.Data;
using BookManagement.Models;
using BookManagement.Services;
using MudBlazor;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Outlined;
    config.SnackbarConfiguration.VisibleStateDuration = 2000;
    config.SnackbarConfiguration.ShowTransitionDuration = 100;
    config.SnackbarConfiguration.HideTransitionDuration = 100;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<IStoryService, StoryService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IUserService, UserService>();


var app = builder.Build();


app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.UseStatusCodePagesWithRedirects("/not-found");

// Главная страница приложения — сразу перенаправляем на список историй.
app.MapGet("/", () => Results.Redirect("/stories"));

// Обрабатывает вход пользователя по имени и паролю.
app.MapPost("/api/login", async (
    [FromForm] string userName,
    [FromForm] string password,
    SignInManager<ApplicationUser> signInManager,
    HttpContext context) =>
{
    // Пытаемся выполнить вход.
    var result = await signInManager.PasswordSignInAsync(
        userName, 
        password, 
        isPersistent: false, 
        lockoutOnFailure: false);

    if (result.Succeeded)
    {
        // Если вход успешен, отправляем пользователя на страницу историй.
        context.Response.Redirect("/stories");
        return;
    }
    
    // Если вход не удался, возвращаем на страницу входа с сообщением об ошибке.
    context.Response.Redirect("/login?error=" + Uri.EscapeDataString("Неверный ник или пароль"));
});

// Завершает текущую сессию пользователя и возвращает его на страницу входа.
app.MapGet("/logout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/login");
});

app.MapRazorComponents<BookManagement.Components.App>()
    .AddInteractiveServerRenderMode();


await DbInitializer.InitializeAsync(app);

app.Run();