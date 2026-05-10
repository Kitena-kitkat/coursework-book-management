using BookManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Data;

/// <summary>
/// Предоставляет методы для инициализации базы данных и заполнения её тестовыми данными.
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Применяет миграции к базе данных и заполняет её тестовыми данными при первом запуске.
    /// </summary>
    /// <param name="app">Экземпляр приложения <see cref="WebApplication"/>.</param>
    /// <returns>Задача асинхронной инициализации базы данных.</returns>
    public static async Task InitializeAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        
        try
        {
            var dbFactory = services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            await using var context = await dbFactory.CreateDbContextAsync();
            
            // Автоматически применяем миграции
            await context.Database.MigrateAsync();
            
            // Заполняем тестовыми данными
            await SeedDataAsync(context, services);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Ошибка при инициализации БД");
            throw;
        }
    }

    /// <summary>
    /// Создаёт тестовые данные для пользователей, историй, отзывов и оценок, если база данных пуста.
    /// </summary>
    /// <param name="context">Контекст базы данных.</param>
    /// <param name="services">Провайдер зависимостей приложения.</param>
    /// <returns>Задача асинхронного заполнения базы данных.</returns>
    private static async Task SeedDataAsync(ApplicationDbContext context, IServiceProvider services)
    {
        if (await context.Users.AnyAsync())
            return;

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // 5 пользователей
        var user1 = new ApplicationUser { UserName = "reader1", Email = "reader1@example.com", DateOfRegistration = DateTime.UtcNow.AddDays(-30) };
        await userManager.CreateAsync(user1, "password123");

        var user2 = new ApplicationUser { UserName = "booklover", Email = "booklover@example.com", DateOfRegistration = DateTime.UtcNow.AddDays(-25) };
        await userManager.CreateAsync(user2, "password123");

        var user3 = new ApplicationUser { UserName = "litfan", Email = "litfan@example.com", DateOfRegistration = DateTime.UtcNow.AddDays(-20) };
        await userManager.CreateAsync(user3, "password123");

        var user4 = new ApplicationUser { UserName = "critic", Email = "critic@example.com", DateOfRegistration = DateTime.UtcNow.AddDays(-15) };
        await userManager.CreateAsync(user4, "password123");

        var user5 = new ApplicationUser { UserName = "bookworm", Email = "bookworm@example.com", DateOfRegistration = DateTime.UtcNow.AddDays(-7) };
        await userManager.CreateAsync(user5, "password123");

        var stories = new List<Story>
        {
            new() { BookName = "Мастер и Маргарита", ShortDescription = "Мистический роман о добре и зле, любви и творчестве", FullDescription = "Однажды в Москве появляется загадочный иностранец Воланд со своей свитой. Он оказывается самим дьяволом, прибывшим на ежегодный бал. В это же время развивается история любви Мастера и Маргариты.", BookRating = 1, UserId = user1.Id, CreatedAt = DateTime.UtcNow.AddDays(-20) },
            new() { BookName = "Преступление и наказание", ShortDescription = "Психологический роман о природе преступления и муках совести", FullDescription = "Бедный студент Раскольников решается на убийство старухи-процентщицы. После преступления начинается его тяжёлый путь к раскаянию. Достоевский исследует психологию преступника.", BookRating = 3, UserId = user1.Id, CreatedAt = DateTime.UtcNow.AddDays(-17) },
            new() { BookName = "Три товарища", ShortDescription = "История дружбы и любви в послевоенной Германии", FullDescription = "Три друга работают в авторемонтной мастерской. Робби влюбляется в Пат. Роман показывает жизнь «потерянного поколения» в Германии 1920-х годов.", BookRating = 4, UserId = user2.Id, CreatedAt = DateTime.UtcNow.AddDays(-14) },
            new() { BookName = "1984", ShortDescription = "Антиутопия о тотальном контроле государства над личностью", FullDescription = "В мире, где Большой Брат следит за каждым, Уинстон Смит пытается сохранить свободу мысли. Он ведёт тайный дневник и влюбляется в Джулию.", BookRating = 5, UserId = user3.Id, CreatedAt = DateTime.UtcNow.AddDays(-10) },
            new() { BookName = "Маленький принц", ShortDescription = "Философская сказка о самом главном в жизни", FullDescription = "Лётчик встречает в пустыне маленького мальчика с другой планеты. Маленький принц рассказывает о своём путешествии. «Самого главного глазами не увидишь — зорко одно лишь сердце».", BookRating = 5, UserId = user2.Id, CreatedAt = DateTime.UtcNow.AddDays(-5) }
        };

        await context.Stories.AddRangeAsync(stories);
        await context.SaveChangesAsync();

        var reviews = new List<Review>
        {

            new() { StoryId = stories[1].Id, AuthorId = user2.Id, Text = "Достоевский — гений! Отличный анализ.", DateOfCreation = DateTime.UtcNow.AddDays(-15) },
            new() { StoryId = stories[1].Id, AuthorId = user4.Id, Text = "Раскольников — один из самых глубоких персонажей.", DateOfCreation = DateTime.UtcNow.AddDays(-12) },
            new() { StoryId = stories[1].Id, AuthorId = user5.Id, Text = "Читал три раза, каждый раз нахожу новое.", DateOfCreation = DateTime.UtcNow.AddDays(-8) },

            new() { StoryId = stories[2].Id, AuthorId = user1.Id, Text = "Ремарк прекрасен. Одна из лучших книг о дружбе.", DateOfCreation = DateTime.UtcNow.AddDays(-12) },
            new() { StoryId = stories[2].Id, AuthorId = user3.Id, Text = "Очень трогательная история.", DateOfCreation = DateTime.UtcNow.AddDays(-9) },

            new() { StoryId = stories[3].Id, AuthorId = user1.Id, Text = "1984 актуален как никогда!", DateOfCreation = DateTime.UtcNow.AddDays(-8) },
            new() { StoryId = stories[3].Id, AuthorId = user2.Id, Text = "Оруэлл предвидел многое. Жутко читать.", DateOfCreation = DateTime.UtcNow.AddDays(-6) },

            new() { StoryId = stories[4].Id, AuthorId = user1.Id, Text = "Маленький принц — книга на все времена.", DateOfCreation = DateTime.UtcNow.AddDays(-4) },
            new() { StoryId = stories[4].Id, AuthorId = user3.Id, Text = "Читаю детям. Учит любви и дружбе.", DateOfCreation = DateTime.UtcNow.AddDays(-3) },
            new() { StoryId = stories[4].Id, AuthorId = user5.Id, Text = "Самое главное — глазами не увидишь.", DateOfCreation = DateTime.UtcNow.AddDays(-1) }
        };

        await context.Reviews.AddRangeAsync(reviews);
        await context.SaveChangesAsync();
        
        var storyLikes = new List<StoryLike>
        {
            new() { StoryId = stories[1].Id, UserId = user2.Id, IsLiked = true },
            new() { StoryId = stories[1].Id, UserId = user3.Id, IsLiked = true },
            new() { StoryId = stories[1].Id, UserId = user4.Id, IsLiked = true },
            
            new() { StoryId = stories[3].Id, UserId = user1.Id, IsLiked = true },
            new() { StoryId = stories[3].Id, UserId = user2.Id, IsLiked = true },
            new() { StoryId = stories[3].Id, UserId = user4.Id, IsLiked = true },
            new() { StoryId = stories[3].Id, UserId = user5.Id, IsLiked = false },
            
            new() { StoryId = stories[4].Id, UserId = user2.Id, IsLiked = true },
            new() { StoryId = stories[4].Id, UserId = user3.Id, IsLiked = true },
            new() { StoryId = stories[4].Id, UserId = user4.Id, IsLiked = false },
            new() { StoryId = stories[4].Id, UserId = user5.Id, IsLiked = true },
        };

        await context.StoryLikes.AddRangeAsync(storyLikes);
        await context.SaveChangesAsync();

        var reviewLikes = new List<ReviewLike>
        {
            new() { ReviewId = reviews[0].Id, UserId = user1.Id, IsLiked = true },
            new() { ReviewId = reviews[0].Id, UserId = user3.Id, IsLiked = true },
            new() { ReviewId = reviews[1].Id, UserId = user1.Id, IsLiked = true },
            new() { ReviewId = reviews[2].Id, UserId = user5.Id, IsLiked = true },
            
            new() { ReviewId = reviews[3].Id, UserId = user1.Id, IsLiked = true },
            new() { ReviewId = reviews[3].Id, UserId = user3.Id, IsLiked = true },
            new() { ReviewId = reviews[4].Id, UserId = user2.Id, IsLiked = true },
            new() { ReviewId = reviews[5].Id, UserId = user4.Id, IsLiked = true },
            
            new() { ReviewId = reviews[6].Id, UserId = user2.Id, IsLiked = true },
            new() { ReviewId = reviews[7].Id, UserId = user4.Id, IsLiked = true },
            
            new() { ReviewId = reviews[8].Id, UserId = user3.Id, IsLiked = false },
            new() { ReviewId = reviews[8].Id, UserId = user5.Id, IsLiked = true },
            
            new() { ReviewId = reviews[9].Id, UserId = user2.Id, IsLiked = true },
            new() { ReviewId = reviews[9].Id, UserId = user4.Id, IsLiked = false },
            new() { ReviewId = reviews[9].Id, UserId = user1.Id, IsLiked = true }
        };

        await context.ReviewLikes.AddRangeAsync(reviewLikes);
        await context.SaveChangesAsync();
    }
}