using BookManagement.Models;

namespace BookManagement.Services;

/// <summary>
/// Интерфейс сервиса для работы с пользователями.
/// Предоставляет методы для получения информации о пользователях из базы данных.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Асинхронно получает пользователя по его идентификатору.
    /// </summary>
    /// <param name="userId">Уникальный идентификатор пользователя.</param>
    /// <returns>Объект <see cref="ApplicationUser"/> если пользователь найден, иначе null.</returns>
    Task<ApplicationUser?> GetUserByIdAsync(string userId);
}