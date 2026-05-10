using BookManagement.Models;

namespace BookManagement.Services;

/// <summary>
/// Интерфейс сервиса для работы с отзывами.
/// Предоставляет методы для управления отзывами на истории и лайками на отзывы.
/// </summary>
public interface IReviewService
{
    /// <summary>
    /// Асинхронно получает все отзывы для конкретной истории.
    /// Отзывы упорядочиваются по количеству лайков в убывающем порядке,
    /// затем по дате создания от новых к старым.
    /// </summary>
    /// <param name="storyId">Идентификатор истории.</param>
    /// <returns>Список отзывов для указанной истории.</returns>
    Task<List<Review>> GetReviewsByStoryIdAsync(int storyId);

    /// <summary>
    /// Асинхронно создаёт новый отзыв.
    /// </summary>
    /// <param name="review">Объект отзыва для создания.</param>
    /// <returns>Созданный объект отзыва.</returns>
    Task<Review> CreateReviewAsync(Review review);

    /// <summary>
    /// Асинхронно обновляет текст отзыва.
    /// Может обновлять только автор отзыва.
    /// </summary>
    /// <param name="review">Объект отзыва с обновлённым текстом.</param>
    /// <param name="userId">Идентификатор пользователя, запрашивающего обновление.</param>
    /// <returns>true если обновление успешно, false если отзыв не найден или прав нет.</returns>
    Task<bool> UpdateReviewAsync(Review review, string userId);

    /// <summary>
    /// Асинхронно удаляет отзыв.
    /// Может удалить только автор отзыва.
    /// </summary>
    /// <param name="reviewId">Идентификатор отзыва для удаления.</param>
    /// <param name="userId">Идентификатор пользователя, запрашивающего удаление.</param>
    /// <returns>true если удаление успешно, false если отзыв не найден или прав нет.</returns>
    Task<bool> DeleteReviewAsync(int reviewId, string userId);

    /// <summary>
    /// Асинхронно переключает лайк/дизлайк пользователя для отзыва.
    /// Если isLiked равно null, удаляет существующую оценку.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="reviewId">Идентификатор отзыва.</param>
    /// <param name="isLiked">true для лайка, false для дизлайка, null для удаления оценки.</param>
    Task ToggleReviewLikeAsync(string userId, int reviewId, bool? isLiked);

    /// <summary>
    /// Асинхронно получает оценку пользователя для отзыва.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="reviewId">Идентификатор отзыва.</param>
    /// <returns>true если это лайк, false если дизлайк, null если оценки нет.</returns>
    Task<bool?> GetUserReviewLikeAsync(string userId, int reviewId);
}