namespace BookManagement.Models;

public class Story
{
    public int Id { get; set; }
    public string BookName { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string FullDescription { get; set; } = string.Empty;
    public int BookRating { get; set; } // 1-5
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? Author { get; set; }
    
    public ICollection<StoryLike>? Likes { get; set; }
    public ICollection<Review>? Reviews { get; set; }
    public ICollection<FavoriteStory>? FavoritedBy { get; set; }
}