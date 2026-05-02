namespace BookManagement.Models;

public class StoryLike
{
    public int Id { get; set; }
    public int StoryId { get; set; }
    public Story? Story { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
    public bool IsLiked { get; set; } // true = лайк, false = дизлайк
}