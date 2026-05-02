using BookManagement.Models;

namespace BookManagement.Services;

public interface IStoryService
{
    Task<List<Story>> GetAllStoriesAsync();
    Task<Story?> GetStoryByIdAsync(int id);
    Task<Story> CreateStoryAsync(Story story);
    Task<bool> UpdateStoryAsync(Story story);
    Task<bool> DeleteStoryAsync(int storyId);
    Task<bool> IsFavoriteAsync(string userId, int storyId);
    Task ToggleFavoriteAsync(string userId, int storyId);
    Task<List<int>> GetFavoriteStoryIdsAsync(string userId);
    Task<int> GetLikeCountAsync(int storyId, bool isLiked);
    Task ToggleStoryLikeAsync(string userId, int storyId, bool isLiked);
}