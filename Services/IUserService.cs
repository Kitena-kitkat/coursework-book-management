using BookManagement.Models;

namespace BookManagement.Services;

public interface IUserService
{
    Task<ApplicationUser?> GetUserByIdAsync(string userId);
}