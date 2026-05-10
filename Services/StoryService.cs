using Microsoft.EntityFrameworkCore;
using BookManagement.Data;
using BookManagement.Models;

namespace BookManagement.Services;

/// <summary>
/// Реализация сервиса для работы с историями/рассказами.
/// Предоставляет методы для управления историями, избранным, лайками и статистикой.
/// </summary>
public class StoryService : IStoryService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="StoryService"/>.
    /// </summary>
    /// <param name="dbFactory">Фабрика контекста базы данных.</param>
    public StoryService(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    /// <summary>
    /// Асинхронно получает все истории, упорядоченные по дате создания в убывающем порядке.
    /// </summary>
    /// <returns>Список всех историй с информацией об авторе.</returns>
    public async Task<List<Story>> GetAllStoriesAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Stories
            .Include(s => s.Author)
            .OrderByDescending(s => s.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Асинхронно получает историю по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор истории.</param>
    /// <returns>Объект <see cref="Story"/> с информацией об авторе и лайках, или null если не найдена.</returns>
    public async Task<Story?> GetStoryByIdAsync(int id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Stories
            .Include(s => s.Author)
            .Include(s => s.Likes)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    /// <summary>
    /// Асинхронно создаёт новую историю.
    /// Проверяет корректность рейтинга (должен быть в диапазоне от 1 до 5).
    /// </summary>
    /// <param name="story">Объект истории для создания.</param>
    /// <returns>Созданный объект истории.</returns>
    /// <exception cref="ArgumentException">Выбросывается, если рейтинг находится вне диапазона 1-5.</exception>
    public async Task<Story> CreateStoryAsync(Story story)
    {
        if (story.BookRating > 5 || story.BookRating < 1)
        {
            throw new ArgumentException("Рейтинг должен быть от 1 до 5");
        }
        await using var db = await _dbFactory.CreateDbContextAsync();
        db.Stories.Add(story);
        await db.SaveChangesAsync();
        return story;
    }

    /// <summary>
    /// Асинхронно обновляет информацию об истории.
    /// Может обновлять только автор истории. Обновляются: название, короткое описание, полное описание и рейтинг.
    /// </summary>
    /// <param name="story">Объект истории с обновлёнными данными.</param>
    /// <param name="userId">Идентификатор пользователя, запрашивающего обновление.</param>
    /// <returns>true если обновление выполнено успешно, false если история не найдена или пользователь не является автором.</returns>
    public async Task<bool> UpdateStoryAsync(Story story, string userId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var existing = await db.Stories.FindAsync(story.Id);
    
        if (existing == null) return false;
        if (existing.UserId != userId) return false; // Проверка прав
    
        existing.BookName = story.BookName;
        existing.ShortDescription = story.ShortDescription;
        existing.FullDescription = story.FullDescription;
        existing.BookRating = story.BookRating;
    
        await db.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Асинхронно удаляет историю.
    /// Может удалить только автор истории.
    /// </summary>
    /// <param name="storyId">Идентификатор истории для удаления.</param>
    /// <param name="userId">Идентификатор пользователя, запрашивающего удаление.</param>
    /// <returns>true если удаление выполнено успешно, false если история не найдена или пользователь не является автором.</returns>
    public async Task<bool> DeleteStoryAsync(int storyId, string userId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var story = await db.Stories.FindAsync(storyId);
        if (story == null) return false;
        if (story.UserId != userId) return false;
        db.Stories.Remove(story);
        await db.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Асинхронно проверяет, добавлена ли история в избранное пользователем.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="storyId">Идентификатор истории.</param>
    /// <returns>true если история в избранном пользователя, иначе false.</returns>
    public async Task<bool> IsFavoriteAsync(string userId, int storyId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.FavoriteStories
            .AnyAsync(f => f.UserId == userId && f.StoryId == storyId);
    }

    /// <summary>
    /// Асинхронно переключает статус избранной истории.
    /// Добавляет историю в избранное, если её там нет, и удаляет, если она там уже есть.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="storyId">Идентификатор истории.</param>
    public async Task ToggleFavoriteAsync(string userId, int storyId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var existing = await db.FavoriteStories
            .FirstOrDefaultAsync(f => f.UserId == userId && f.StoryId == storyId);
        
        if (existing != null)
            db.FavoriteStories.Remove(existing);
        else
            db.FavoriteStories.Add(new FavoriteStory { UserId = userId, StoryId = storyId });
        
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Асинхронно получает список идентификаторов всех избранных историй пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns>Список идентификаторов избранных историй пользователя.</returns>
    public async Task<List<int>> GetFavoriteStoryIdsAsync(string userId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.FavoriteStories
            .Where(f => f.UserId == userId)
            .Select(f => f.StoryId)
            .ToListAsync();
    }

    /// <summary>
    /// Асинхронно получает количество лайков или дизлайков для истории.
    /// </summary>
    /// <param name="storyId">Идентификатор истории.</param>
    /// <param name="isLiked">true для подсчёта лайков, false для подсчёта дизлайков.</param>
    /// <returns>Количество лайков или дизлайков для указанной истории.</returns>
    public async Task<int> GetLikeCountAsync(int storyId, bool isLiked)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.StoryLikes
            .CountAsync(l => l.StoryId == storyId && l.IsLiked == isLiked);
    }

    /// <summary>
    /// Асинхронно переключает лайк/дизлайк пользователя для истории.
    /// Если isLiked равно null, удаляет существующую оценку.
    /// Если существует такая же оценка, удаляет её.
    /// Если существует другая оценка, обновляет её на новую.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="storyId">Идентификатор истории.</param>
    /// <param name="isLiked">true для лайка, false для дизлайка, null для удаления оценки.</param>
    public async Task ToggleStoryLikeAsync(string userId, int storyId, bool? isLiked)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var existing = await db.StoryLikes
            .FirstOrDefaultAsync(l => l.UserId == userId && l.StoryId == storyId);
    
        if (isLiked == null)
        {
            
            if (existing != null)
            {
                db.StoryLikes.Remove(existing);
                await db.SaveChangesAsync();
            }
            return;
        }
    
        if (existing != null)
        {
            if (existing.IsLiked == isLiked)
                db.StoryLikes.Remove(existing);  
            else
            {
                existing.IsLiked = isLiked.Value;  
                db.StoryLikes.Update(existing);
            }
        }
        else
        {
            db.StoryLikes.Add(new StoryLike { UserId = userId, StoryId = storyId, IsLiked = isLiked.Value });
        }
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Асинхронно получает оценку пользователя для истории.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="storyId">Идентификатор истории.</param>
    /// <returns>true если это лайк, false если это дизлайк, null если оценки нет.</returns>
    public async Task<bool?> GetUserStoryLikeAsync(string userId, int storyId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var like = await db.StoryLikes
            .FirstOrDefaultAsync(l => l.UserId == userId && l.StoryId == storyId);
        return like?.IsLiked; 
    }
}