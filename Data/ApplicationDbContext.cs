using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookManagement.Models;

namespace BookManagement.Data;

/// <summary>
/// Контекст базы данных приложения Book Management.
/// Наследует IdentityDbContext для управления пользователями и их ролями.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>
    /// Инициализирует новый экземпляр класса ApplicationDbContext.
    /// </summary>
    /// <param name="options">Параметры конфигурации для контекста базы данных.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    /// <summary>
    /// Получает или устанавливает набор рассказов в базе данных.
    /// </summary>
    public DbSet<Story> Stories { get; set; }

    /// <summary>
    /// Получает или устанавливает набор лайков рассказов в базе данных.
    /// </summary>
    public DbSet<StoryLike> StoryLikes { get; set; }

    /// <summary>
    /// Получает или устанавливает набор отзывов в базе данных.
    /// </summary>
    public DbSet<Review> Reviews { get; set; }

    /// <summary>
    /// Получает или устанавливает набор лайков отзывов в базе данных.
    /// </summary>
    public DbSet<ReviewLike> ReviewLikes { get; set; }

    /// <summary>
    /// Получает или устанавливает набор избранных рассказов в базе данных.
    /// </summary>
    public DbSet<FavoriteStory> FavoriteStories { get; set; }

    /// <summary>
    /// Переопределяет метод для конфигурации модели данных при создании контекста.
    /// Устанавливает связи между сущностями и ограничения уникальности.
    /// </summary>
    /// <param name="builder">Генератор модели для конфигурации сущностей.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Конфигурация связи "один автор - много рассказов".
        // При удалении автора все его рассказы также удаляются.
        builder.Entity<Story>()
            .HasOne(s => s.Author)
            .WithMany(a => a.Stories)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Конфигурация ограничения уникальности для лайков рассказов.
        // Один пользователь может лайкнуть рассказ только один раз.
        builder.Entity<StoryLike>()
            .HasIndex(sl => new { sl.StoryId, sl.UserId })
            .IsUnique();

        // Конфигурация связи "один автор - много отзывов".
        // При удалении автора все его отзывы также удаляются.
        builder.Entity<Review>()
            .HasOne(r => r.Author)
            .WithMany(a => a.Reviews)
            .HasForeignKey(r => r.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Конфигурация ограничения уникальности для лайков отзывов.
        // Один пользователь может лайкнуть отзыв только один раз.
        builder.Entity<ReviewLike>()
            .HasIndex(rl => new { rl.ReviewId, rl.UserId })
            .IsUnique();

        // Конфигурация ограничения уникальности для избранных рассказов.
        // Один пользователь может добавить рассказ в избранное только один раз.
        builder.Entity<FavoriteStory>()
            .HasIndex(fs => new { fs.StoryId, fs.UserId })
            .IsUnique();
    }
}