namespace BookManagement.Models;

/// <summary>
/// Представляет лайк или дизлайк пользователя на историю.
/// </summary>
public class StoryLike
{
    /// <summary>
    /// Получает или задаёт уникальный идентификатор лайка.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Получает или задаёт идентификатор истории.
    /// </summary>
    public int StoryId { get; set; }

    /// <summary>
    /// Получает или задаёт объект связанной истории.
    /// </summary>
    public Story? Story { get; set; }

    /// <summary>
    /// Получает или задаёт идентификатор пользователя, который поставил лайк/дизлайк.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Получает или задаёт объект связанного пользователя.
    /// </summary>
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// Получает или задаёт значение, указывающее является ли это лайком или дизлайком.
    /// true = лайк, false = дизлайк.
    /// </summary>
    public bool IsLiked { get; set; }
}