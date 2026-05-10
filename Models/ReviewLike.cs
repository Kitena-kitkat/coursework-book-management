namespace BookManagement.Models;

/// <summary>
/// Представляет отметку реакции пользователя на отзыв.
/// </summary>
public class ReviewLike
{
    /// <summary>
    /// Уникальный идентификатор реакции.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Идентификатор отзыва, к которому относится реакция.
    /// </summary>
    public int ReviewId { get; set; }

    /// <summary>
    /// Связанный отзыв.
    /// </summary>
    public Review? Review { get; set; }

    /// <summary>
    /// Идентификатор пользователя, поставившего реакцию.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Связанный пользователь.
    /// </summary>
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// Признак того, что реакция является положительной или отрицательной.
    /// </summary>
    public bool IsLiked { get; set; }
}