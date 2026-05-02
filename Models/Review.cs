namespace BookManagement.Models;

public class Review
{
    public int Id { get; set; }
    public int StoryId { get; set; }
    public Story? Story { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public ApplicationUser? Author { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime DateOfCreation { get; set; } = DateTime.UtcNow;
    
    public ICollection<ReviewLike>? Likes { get; set; }
}