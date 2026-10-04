using ECommerce.OrderService.Models;

namespace ECommerce.OrderService.Interfaces
{
    public interface IOrderService
    {
        Task<Order> PlaceOrderAsync(int userId);
        Task<Order?> GetByIdAsync(int id);
        Task<List<Order>> GetAllAsync();
        Task UpdateStatusAsync(int id, string status);
        Task<List<Order>> GetMyOrdersAsync(int userId);
    }
}