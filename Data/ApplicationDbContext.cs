using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookManagement.Models;

namespace BookManagement.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Story> Stories { get; set; }
    public DbSet<StoryLike> StoryLikes { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<ReviewLike> ReviewLikes { get; set; }
    public DbSet<FavoriteStory> FavoriteStories { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        

        builder.Entity<Story>()
            .HasOne(s => s.Author)
            .WithMany(a => a.Stories)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // StoryLike: один пользователь может лайкнуть рассказ только один раз
        builder.Entity<StoryLike>()
            .HasIndex(sl => new { sl.StoryId, sl.UserId })
            .IsUnique();

        // Review
        builder.Entity<Review>()
            .HasOne(r => r.Author)
            .WithMany(a => a.Reviews)
            .HasForeignKey(r => r.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        // ReviewLike: один пользователь — один лайк на отзыв
        builder.Entity<ReviewLike>()
            .HasIndex(rl => new { rl.ReviewId, rl.UserId })
            .IsUnique();

        // FavoriteStory: один пользователь — одна запись в избранном на рассказ
        builder.Entity<FavoriteStory>()
            .HasIndex(fs => new { fs.StoryId, fs.UserId })
            .IsUnique();
    }
}