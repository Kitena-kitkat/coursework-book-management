namespace BookManagement.Models;
using System.ComponentModel.DataAnnotations;

public class Story
{
    public int Id { get; set; }
    [MaxLength(500, ErrorMessage = "Информация о книге не может превышать 500 символов")]
    public string BookName { get; set; } = string.Empty;
    [MaxLength(1000, ErrorMessage = "Короткое описание впечатлений не может превышать 1000 символов")]
    public string ShortDescription { get; set; } = string.Empty;
    [MaxLength(6000, ErrorMessage = "Полное описание впечатлений не может превышать 6000 символов")]
    public string FullDescription { get; set; } = string.Empty;
    [Range(1, 5, ErrorMessage = "Оценка должна быть от 1 до 5")]
    public int BookRating { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? Author { get; set; }
    
    public ICollection<StoryLike>? Likes { get; set; }
    public ICollection<Review>? Reviews { get; set; }
    public ICollection<FavoriteStory>? FavoritedBy { get; set; }
}