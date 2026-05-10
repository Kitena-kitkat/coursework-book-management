using BookManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Data;

public static class DbInitializer
{
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

    private static async Task SeedDataAsync(ApplicationDbContext context, IServiceProvider services)
    {
        // Если уже есть данные - выходим
        if (await context.Users.AnyAsync())
            return;

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // Создаём тестовых пользователей
        var user1 = new ApplicationUser
        {
            UserName = "reader1",
            Email = "reader1@example.com",
            DateOfRegistration = DateTime.UtcNow.AddDays(-30)
        };
        await userManager.CreateAsync(user1, "password123");

        var user2 = new ApplicationUser
        {
            UserName = "booklover",
            Email = "booklover@example.com",
            DateOfRegistration = DateTime.UtcNow.AddDays(-15)
        };
        await userManager.CreateAsync(user2, "password123");

        // Создаём тестовые рассказы
        var stories = new List<Story>
        {
            new()
            {
                BookName = "Мастер и Маргарита",
                ShortDescription = "Мистический роман о добре и зле, любви и творчестве",
                FullDescription = "Однажды весной в Москве появляется загадочный иностранец Воланд со своей свитой. " +
                                  "Он оказывается самим дьяволом, прибывшим на ежегодный бал. В это же время развивается " +
                                  "история любви Мастера и Маргариты. Роман поражает глубиной философских размышлений " +
                                  "и сатирическим изображением московского общества 1930-х годов.",
                BookRating = 5,
                UserId = user1.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new()
            {
                BookName = "Преступление и наказание",
                ShortDescription = "Психологический роман о природе преступления и муках совести",
                FullDescription = "Бедный студент Родион Раскольников решается на убийство старухи-процентщицы, " +
                                  "считая себя «право имеющим». После преступления начинается его тяжёлый путь " +
                                  "к раскаянию. Достоевский мастерски исследует психологию преступника и показывает, " +
                                  "что настоящее наказание — это муки совести.",
                BookRating = 5,
                UserId = user1.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new()
            {
                BookName = "Три товарища",
                ShortDescription = "История дружбы и любви в послевоенной Германии",
                FullDescription = "Три друга — Робби, Кестер и Ленц — работают в авторемонтной мастерской. " +
                                  "Робби влюбляется в Пат, девушку из богатой семьи. Роман показывает жизнь " +
                                  "«потерянного поколения» в Германии 1920-х годов. Это невероятно трогательная " +
                                  "история о дружбе, любви и ценности каждого мгновения жизни.",
                BookRating = 4,
                UserId = user2.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new()
            {
                BookName = "1984",
                ShortDescription = "Антиутопия о тотальном контроле государства над личностью",
                FullDescription = "В мире, где Большой Брат следит за каждым, Уинстон Смит пытается сохранить " +
                                  "человечность и свободу мысли. Он ведёт тайный дневник и влюбляется в Джулию. " +
                                  "Но Партия не прощает инакомыслия... Книга-предупреждение, которая остаётся " +
                                  "актуальной и сегодня.",
                BookRating = 5,
                UserId = user2.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new()
            {
                BookName = "Маленький принц",
                ShortDescription = "Философская сказка о самом главном в жизни",
                FullDescription = "Лётчик встречает в пустыне маленького мальчика с другой планеты. " +
                                  "Маленький принц рассказывает о своём путешествии и встречах с разными людьми. " +
                                  "Эта книга напоминает, что «самого главного глазами не увидишь — зорко одно лишь сердце».",
                BookRating = 5,
                UserId = user1.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        await context.Stories.AddRangeAsync(stories);
        await context.SaveChangesAsync();

        // Создаём отзывы
        var reviews = new List<Review>
        {
            new()
            {
                StoryId = stories[0].Id,
                AuthorId = user2.Id,
                Text = "Отличный обзор! Тоже люблю этот роман. Особенно впечатлила сцена бала у Воланда.",
                DateOfCreation = DateTime.UtcNow.AddDays(-18)
            },
            new()
            {
                StoryId = stories[1].Id,
                AuthorId = user2.Id,
                Text = "Достоевский — гений психологического романа. Отличный анализ!",
                DateOfCreation = DateTime.UtcNow.AddDays(-12)
            },
            new()
            {
                StoryId = stories[2].Id,
                AuthorId = user1.Id,
                Text = "Ремарк прекрасен. «Три товарища» — одна из лучших книг о дружбе.",
                DateOfCreation = DateTime.UtcNow.AddDays(-8)
            }
        };

        await context.Reviews.AddRangeAsync(reviews);
        await context.SaveChangesAsync();
    }
}