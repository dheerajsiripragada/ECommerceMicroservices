using ECommerce.UserService.Models;

namespace ECommerce.UserService.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<User?> LoginAsync(string email, string password);
    }
}