using Microsoft.AspNetCore.Identity;

namespace BookManagement.Models
{
    // Наследуемся от IdentityUser, чтобы получить готовую инфраструктуру
    public class ApplicationUser : IdentityUser
    {

        // Дата регистрации - заполняется автоматически при создании
        public DateTime DateOfRegistration { get; set; } = DateTime.UtcNow;
        public ICollection<Story>? Stories { get; set; }
        public ICollection<StoryLike>? StoryLikes { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<ReviewLike>? ReviewLikes { get; set; }
        public ICollection<FavoriteStory>? FavoriteStories { get; set; }

    }
}