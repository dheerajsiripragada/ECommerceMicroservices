using ECommerce.OrderService.Models;

namespace ECommerce.OrderService.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllAsync();
        Task<List<Order>> GetByUserIdAsync(int userId);
        Task<Order?> GetByIdAsync(int id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
    }
}