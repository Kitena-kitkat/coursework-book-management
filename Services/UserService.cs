using Microsoft.EntityFrameworkCore;
using BookManagement.Data;
using BookManagement.Models;

namespace BookManagement.Services;

public class UserService : IUserService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
    public UserService(IDbContextFactory<ApplicationDbContext> dbFactory) => _dbFactory = dbFactory;

    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
    }
}