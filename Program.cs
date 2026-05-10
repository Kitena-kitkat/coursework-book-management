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

app.MapGet("/", () => Results.Redirect("/stories"));

app.MapPost("/api/login", async (
    [FromForm] string UserName,
    [FromForm] string Password,
    SignInManager<ApplicationUser> signInManager,
    HttpContext context) =>
{
    var result = await signInManager.PasswordSignInAsync(
        UserName, 
        Password, 
        isPersistent: false, 
        lockoutOnFailure: false);

    if (result.Succeeded)
    {
        context.Response.Redirect("/stories");
        return;
    }
    
    context.Response.Redirect("/login?error=" + Uri.EscapeDataString("Неверный ник или пароль"));
});

app.MapGet("/logout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/login");
});

app.MapRazorComponents<BookManagement.Components.App>()
    .AddInteractiveServerRenderMode();


// Применяем миграции и заполняем БД при каждом запуске
await DbInitializer.InitializeAsync(app);

app.Run();