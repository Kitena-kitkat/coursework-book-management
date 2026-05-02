namespace BookManagement.Models;

public class ReviewLike
{
    public int Id { get; set; }
    public int ReviewId { get; set; }
    public Review? Review { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
    public bool IsLiked { get; set; }
}