using ECommerce.OrderService.Models;

namespace ECommerce.OrderService.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetByUserIdAsync(int userId);
        Task AddAsync(Cart cart);
        Task UpdateAsync(Cart cart);
        Task DeleteAsync(int id);
    }
}