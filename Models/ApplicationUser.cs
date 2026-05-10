using Microsoft.AspNetCore.Identity;

namespace BookManagement.Models
{
    /// <summary>
    /// Представляет пользователя приложения, расширяющего функциональность IdentityUser.
    /// </summary>
    /// <remarks>
    /// Класс ApplicationUser наследует функциональность аутентификации и авторизации от IdentityUser,
    /// добавляя при этом дополнительные свойства для управления историями и рецензиями пользователя.
    /// </remarks>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Получает или задает дату регистрации пользователя в приложении.
        /// </summary>
        /// <value>
        /// Значение типа <see cref="DateTime"/> в формате UTC, автоматически устанавливаемое при создании пользователя.
        /// </value>
        public DateTime DateOfRegistration { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Получает или задает коллекцию историй, созданных пользователем.
        /// </summary>
        /// <value>
        /// Коллекция объектов <see cref="Story"/>, связанных с данным пользователем, или null если нет связанных историй.
        /// </value>
        public ICollection<Story>? Stories { get; set; }

        /// <summary>
        /// Получает или задает коллекцию рецензий, написанных пользователем.
        /// </summary>
        /// <value>
        /// Коллекция объектов <see cref="Review"/>, связанных с данным пользователем, или null если нет написанных рецензий.
        /// </value>
        public ICollection<Review>? Reviews { get; set; }

    }
}