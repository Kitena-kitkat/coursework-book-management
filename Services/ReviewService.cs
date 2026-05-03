using Microsoft.EntityFrameworkCore;
using BookManagement.Data;
using BookManagement.Models;

namespace BookManagement.Services;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _db;

    public ReviewService(ApplicationDbContext db) => _db = db;

    public async Task<List<Review>> GetReviewsByStoryIdAsync(int storyId) =>
        await _db.Reviews
            .Include(r => r.Author)
            .Include(r => r.Likes)
            .Where(r => r.StoryId == storyId)
            .OrderByDescending(r => r.Likes!.Count(l => l.IsLiked))  //  Сначала больше лайков
            .ThenByDescending(r => r.DateOfCreation)                  //  Потом новее
            .ToListAsync();

    public async Task<Review> CreateReviewAsync(Review review)
    {
        review.DateOfCreation = DateTime.UtcNow;
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();
        return review;
    }

    public async Task<bool> UpdateReviewAsync(Review review, string userId)
    {
        var existing = await _db.Reviews.FindAsync(review.Id);
        if (existing == null) return false;
        if (existing.AuthorId != userId) return false;
        existing.Text = review.Text;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteReviewAsync(int reviewId, string userId)
    {
        var review = await _db.Reviews.FindAsync(reviewId);
        if (review == null) return false;
        if (review.AuthorId != userId) return false;
        _db.Reviews.Remove(review);
        await _db.SaveChangesAsync();
        return true;
    }
    

    public async Task ToggleReviewLikeAsync(string userId, int reviewId, bool? isLiked)
    {
        var existing = await _db.ReviewLikes
            .FirstOrDefaultAsync(l => l.UserId == userId && l.ReviewId == reviewId);
    
        if (isLiked == null)
        {
            if (existing != null)
            {
                _db.ReviewLikes.Remove(existing);
                await _db.SaveChangesAsync();
            }
            return;
        }
    
        if (existing != null)
        {
            if (existing.IsLiked == isLiked)
                _db.ReviewLikes.Remove(existing);
            else
            {
                existing.IsLiked = isLiked.Value;
                _db.ReviewLikes.Update(existing);
            }
        }
        else
        {
            _db.ReviewLikes.Add(new ReviewLike { UserId = userId, ReviewId = reviewId, IsLiked = isLiked.Value });
        }
        await _db.SaveChangesAsync();
    }
    
    public async Task<bool?> GetUserReviewLikeAsync(string userId, int reviewId)
    {
        var like = await _db.ReviewLikes
            .FirstOrDefaultAsync(l => l.UserId == userId && l.ReviewId == reviewId);
        return like?.IsLiked;
    }
}