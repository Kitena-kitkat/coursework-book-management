namespace BookManagement.Models;

public class FavoriteStory
{
    public int Id { get; set; }
    public int StoryId { get; set; }
    public Story? Story { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
}