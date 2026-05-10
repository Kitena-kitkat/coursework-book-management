using Microsoft.EntityFrameworkCore;
using BookManagement.Data;
using BookManagement.Models;

namespace BookManagement.Services;

/// <summary>
/// Реализация сервиса для работы с пользователями.
/// Обеспечивает получение информации о пользователях из базы данных.
/// </summary>
public class UserService : IUserService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="UserService"/>.
    /// </summary>
    /// <param name="dbFactory">Фабрика контекста базы данных.</param>
    public UserService(IDbContextFactory<ApplicationDbContext> dbFactory) => _dbFactory = dbFactory;

    /// <summary>
    /// Асинхронно получает пользователя по его идентификатору.
    /// Использует режим отслеживания AsNoTracking для оптимизации.
    /// </summary>
    /// <param name="userId">Уникальный идентификатор пользователя.</param>
    /// <returns>Объект <see cref="ApplicationUser"/> если пользователь найден, иначе null.</returns>
    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
    }
}