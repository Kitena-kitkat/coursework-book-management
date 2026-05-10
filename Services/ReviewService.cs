using Microsoft.EntityFrameworkCore;
using BookManagement.Data;
using BookManagement.Models;

namespace BookManagement.Services;

/// <summary>
/// Реализация сервиса для работы с отзывами.
/// Предоставляет методы для управления отзывами на истории и лайками на отзывы.
/// </summary>
public class ReviewService : IReviewService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="ReviewService"/>.
    /// </summary>
    /// <param name="dbFactory">Фабрика контекста базы данных.</param>
    public ReviewService(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    /// <summary>
    /// Асинхронно получает все отзывы для конкретной истории.
    /// Отзывы упорядочиваются по количеству лайков в убывающем порядке,
    /// затем по дате создания от новых к старым.
    /// </summary>
    /// <param name="storyId">Идентификатор истории.</param>
    /// <returns>Список отзывов для указанной истории, упорядоченный по популярности и времени.</returns>
    public async Task<List<Review>> GetReviewsByStoryIdAsync(int storyId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Reviews
            .Include(r => r.Author)
            .Include(r => r.Likes)
            .Where(r => r.StoryId == storyId)
            .OrderByDescending(r => r.Likes!.Count(l => l.IsLiked)) 
            .ThenByDescending(r => r.DateOfCreation) 
            .ToListAsync();
    }

    /// <summary>
    /// Асинхронно создаёт новый отзыв.
    /// </summary>
    /// <param name="review">Объект отзыва для создания.</param>
    /// <returns>Созданный объект отзыва.</returns>
    public async Task<Review> CreateReviewAsync(Review review)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        db.Reviews.Add(review);
        await db.SaveChangesAsync();
        return review;
    }

    /// <summary>
    /// Асинхронно обновляет текст отзыва.
    /// Может обновлять только автор отзыва.
    /// </summary>
    /// <param name="review">Объект отзыва с обновлённым текстом.</param>
    /// <param name="userId">Идентификатор пользователя, запрашивающего обновление.</param>
    /// <returns>true если обновление выполнено успешно, false если отзыв не найден или пользователь не является автором.</returns>
    public async Task<bool> UpdateReviewAsync(Review review, string userId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var existing = await db.Reviews.FindAsync(review.Id);
        if (existing == null) return false;
        if (existing.AuthorId != userId) return false;
        existing.Text = review.Text;
        await db.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Асинхронно удаляет отзыв.
    /// Может удалить только автор отзыва.
    /// </summary>
    /// <param name="reviewId">Идентификатор отзыва для удаления.</param>
    /// <param name="userId">Идентификатор пользователя, запрашивающего удаление.</param>
    /// <returns>true если удаление выполнено успешно, false если отзыв не найден или пользователь не является автором.</returns>
    public async Task<bool> DeleteReviewAsync(int reviewId, string userId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var review = await db.Reviews.FindAsync(reviewId);
        if (review == null) return false;
        if (review.AuthorId != userId) return false;
        db.Reviews.Remove(review);
        await db.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Асинхронно переключает лайк/дизлайк пользователя для отзыва.
    /// Если isLiked равно null, удаляет существующую оценку.
    /// Если существует такая же оценка, удаляет её.
    /// Если существует другая оценка, обновляет её на новую.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="reviewId">Идентификатор отзыва.</param>
    /// <param name="isLiked">true для лайка, false для дизлайка, null для удаления оценки.</param>
    public async Task ToggleReviewLikeAsync(string userId, int reviewId, bool? isLiked)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var existing = await db.ReviewLikes
            .FirstOrDefaultAsync(l => l.UserId == userId && l.ReviewId == reviewId);
    
        if (isLiked == null)
        {
            if (existing != null)
            {
                db.ReviewLikes.Remove(existing);
                await db.SaveChangesAsync();
            }
            return;
        }
    
        if (existing != null)
        {
            if (existing.IsLiked == isLiked)
                db.ReviewLikes.Remove(existing);
            else
            {
                existing.IsLiked = isLiked.Value;
                db.ReviewLikes.Update(existing);
            }
        }
        else
        {
            db.ReviewLikes.Add(new ReviewLike { UserId = userId, ReviewId = reviewId, IsLiked = isLiked.Value });
        }
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Асинхронно получает оценку пользователя для отзыва.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="reviewId">Идентификатор отзыва.</param>
    /// <returns>true если это лайк, false если это дизлайк, null если оценки нет.</returns>
    public async Task<bool?> GetUserReviewLikeAsync(string userId, int reviewId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var like = await db.ReviewLikes
            .FirstOrDefaultAsync(l => l.UserId == userId && l.ReviewId == reviewId);
        return like?.IsLiked;
    }
}