using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using BookManagement.Data;
using BookManagement.Models;

var builder = WebApplication.CreateBuilder(args);

// Подключаем MudBlazor
builder.Services.AddMudServices();

// Подключаем DbContext с PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Подключаем Identity с нашей моделью пользователя
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Упрощаем требования к паролю для курсовой
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Добавляем поддержку серверных компонентов Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Добавляем аутентификацию и авторизацию
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Стандартный пайплайн
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

app.MapRazorComponents<BookManagement.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();