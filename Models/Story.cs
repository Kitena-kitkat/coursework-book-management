namespace BookManagement.Models;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// История прочтения книги, содержащая сведения о книге, впечатлениях пользователя, рейтинге и связанных сущностях.
/// </summary>
public class Story
{
    /// <summary>
    /// Уникальный идентификатор истории.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название книги и, возможно, автор
    /// </summary>
    [MaxLength(500, ErrorMessage = "Информация о книге не может превышать 500 символов")]
    public string BookName { get; set; } = string.Empty;

    /// <summary>
    /// Короткое описание впечатлений от книги.
    /// </summary>
    [MaxLength(1000, ErrorMessage = "Короткое описание впечатлений не может превышать 1000 символов")]
    public string ShortDescription { get; set; } = string.Empty;

    /// <summary>
    /// Полное описание впечатлений от книги.
    /// </summary>
    [MaxLength(6000, ErrorMessage = "Полное описание впечатлений не может превышать 6000 символов")]
    public string FullDescription { get; set; } = string.Empty;

    /// <summary>
    /// Оценка книги по шкале от 1 до 5.
    /// </summary>
    [Range(1, 5, ErrorMessage = "Оценка должна быть от 1 до 5")]
    public int BookRating { get; set; } 

    /// <summary>
    /// Дата и время создания истории в формате UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Идентификатор пользователя-автора истории.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Автор истории.
    /// </summary>
    public ApplicationUser? Author { get; set; }
    
    /// <summary>
    /// Коллекция лайков, поставленных истории.
    /// </summary>
    public ICollection<StoryLike>? Likes { get; set; }

    /// <summary>
    /// Коллекция отзывов к истории.
    /// </summary>
    public ICollection<Review>? Reviews { get; set; }

    /// <summary>
    /// Коллекция записей о добавлении истории в избранное.
    /// </summary>
    public ICollection<FavoriteStory>? FavoritedBy { get; set; }
}