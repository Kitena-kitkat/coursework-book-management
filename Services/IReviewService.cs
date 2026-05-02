using BookManagement.Models;

namespace BookManagement.Services;

public interface IReviewService
{
    Task<List<Review>> GetReviewsByStoryIdAsync(int storyId);
    Task<Review> CreateReviewAsync(Review review);
    Task<bool> UpdateReviewAsync(Review review);
    Task<bool> DeleteReviewAsync(int reviewId);
    Task<int> GetLikeCountAsync(int reviewId, bool isLiked);
    Task ToggleReviewLikeAsync(string userId, int reviewId, bool isLiked);
    Task<bool> IsReviewLikedByUserAsync(string userId, int reviewId);
}