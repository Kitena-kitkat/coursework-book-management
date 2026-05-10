namespace BookManagement.Models;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Представляет отзыв пользователя к истории.
/// </summary>
public class Review
{
    /// <summary>
    /// Уникальный идентификатор отзыва.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Идентификатор истории, к которой относится отзыв.
    /// </summary>
    public int StoryId { get; set; }
    /// <summary>
    /// Связанная история.
    /// </summary>
    public Story? Story { get; set; }
    /// <summary>
    /// Идентификатор автора отзыва.
    /// </summary>
    public string AuthorId { get; set; } = string.Empty;
    /// <summary>
    /// Автор отзыва.
    /// </summary>
    public ApplicationUser? Author { get; set; }
    /// <summary>
    /// Текст отзыва.
    /// </summary>
    [MaxLength(6000, ErrorMessage = "Текст отзыва не может превышать 6000 символов")]
    public string Text { get; set; } = string.Empty;
    /// <summary>
    /// Дата и время создания отзыва в UTC.
    /// </summary>
    public DateTime DateOfCreation { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Коллекция лайков отзыва.
    /// </summary>
    public ICollection<ReviewLike>? Likes { get; set; }
}