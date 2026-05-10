namespace BookManagement.Models;

/// <summary>
/// Представляет избранную историю пользователя.
/// </summary>
/// <remarks>
/// Класс используется для связи пользователя с историями, которые они добавили в избранное.
/// </remarks>
public class FavoriteStory
{
    /// <summary>
    /// Получает или задает уникальный идентификатор избранной истории.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Получает или задает идентификатор истории.
    /// </summary>
    public int StoryId { get; set; }

    /// <summary>
    /// Получает или задает связанную историю.
    /// </summary>
    public Story? Story { get; set; }

    /// <summary>
    /// Получает или задает идентификатор пользователя.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Получает или задает связанного пользователя.
    /// </summary>
    public ApplicationUser? User { get; set; }
}