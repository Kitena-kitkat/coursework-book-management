using Microsoft.EntityFrameworkCore;
using BookManagement.Data;
using BookManagement.Models;

namespace BookManagement.Services;

public class ReviewService : IReviewService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public ReviewService(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<Review>> GetReviewsByStoryIdAsync(int storyId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Reviews
            .Include(r => r.Author)
            .Include(r => r.Likes)
            .Where(r => r.StoryId == storyId)
            .OrderByDescending(r => r.Likes!.Count(l => l.IsLiked)) //  Сначала больше лайков
            .ThenByDescending(r => r.DateOfCreation) //  Потом новее
            .ToListAsync();
    }

    public async Task<Review> CreateReviewAsync(Review review)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        review.DateOfCreation = DateTime.UtcNow;
        db.Reviews.Add(review);
        await db.SaveChangesAsync();
        return review;
    }

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
    
    public async Task<bool?> GetUserReviewLikeAsync(string userId, int reviewId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var like = await db.ReviewLikes
            .FirstOrDefaultAsync(l => l.UserId == userId && l.ReviewId == reviewId);
        return like?.IsLiked;
    }
}