using Microsoft.AspNetCore.Identity;

namespace BookManagement.Models
{
    // Наследуемся от IdentityUser, чтобы получить готовую инфраструктуру
    public class ApplicationUser : IdentityUser
    {

        // Дата регистрации - заполняется автоматически при создании
        public DateTime DateOfRegistration { get; set; } = DateTime.UtcNow;

    }
}