using BookManagement.Models;

namespace BookManagement.Services;

public interface IStoryService
{
    Task<List<Story>> GetAllStoriesAsync();
    Task<Story?> GetStoryByIdAsync(int id);
    Task<Story> CreateStoryAsync(Story story);
    Task<bool> UpdateStoryAsync(Story story, string userId);
    Task<bool> DeleteStoryAsync(int storyId, string userId);
    Task<bool> IsFavoriteAsync(string userId, int storyId);
    Task ToggleFavoriteAsync(string userId, int storyId);
    Task<List<int>> GetFavoriteStoryIdsAsync(string userId);
    Task<int> GetLikeCountAsync(int storyId, bool isLiked);
    Task ToggleStoryLikeAsync(string userId, int storyId, bool? isLiked);
    Task<bool?> GetUserStoryLikeAsync(string userId, int storyId);
    
}