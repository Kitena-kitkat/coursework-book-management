using Microsoft.EntityFrameworkCore;
using BookManagement.Data;
using BookManagement.Models;

namespace BookManagement.Services;

public class StoryService : IStoryService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public StoryService(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<Story>> GetAllStoriesAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Stories
            .Include(s => s.Author)
            .OrderByDescending(s => s.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Story?> GetStoryByIdAsync(int id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Stories
            .Include(s => s.Author)
            .Include(s => s.Likes)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Story> CreateStoryAsync(Story story)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        db.Stories.Add(story);
        await db.SaveChangesAsync();
        return story;
    }

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

    public async Task<bool> IsFavoriteAsync(string userId, int storyId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.FavoriteStories
            .AnyAsync(f => f.UserId == userId && f.StoryId == storyId);
    }

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

    public async Task<List<int>> GetFavoriteStoryIdsAsync(string userId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.FavoriteStories
            .Where(f => f.UserId == userId)
            .Select(f => f.StoryId)
            .ToListAsync();
    }

    public async Task<int> GetLikeCountAsync(int storyId, bool isLiked)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.StoryLikes
            .CountAsync(l => l.StoryId == storyId && l.IsLiked == isLiked);
    }

    public async Task ToggleStoryLikeAsync(string userId, int storyId, bool isLiked)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var existing = await db.StoryLikes
            .FirstOrDefaultAsync(l => l.UserId == userId && l.StoryId == storyId);
        
        if (existing != null)
        {
            if (existing.IsLiked == isLiked)
                db.StoryLikes.Remove(existing);
            else
                existing.IsLiked = isLiked;
        }
        else
        {
            db.StoryLikes.Add(new StoryLike { UserId = userId, StoryId = storyId, IsLiked = isLiked });
        }
        await db.SaveChangesAsync();
    }
}