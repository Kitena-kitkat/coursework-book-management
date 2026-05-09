namespace BookManagement.Models;
using System.ComponentModel.DataAnnotations;

public class Review
{
    public int Id { get; set; }
    public int StoryId { get; set; }
    public Story? Story { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public ApplicationUser? Author { get; set; }
    [MaxLength(6000, ErrorMessage = "Текст отзыва не может превышать 6000 символов")]
    public string Text { get; set; } = string.Empty;
    public DateTime DateOfCreation { get; set; } = DateTime.UtcNow;
    
    public ICollection<ReviewLike>? Likes { get; set; }
}