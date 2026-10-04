using ECommerce.OrderService.Models;

namespace ECommerce.OrderService.Interfaces
{
    public interface ICartService
    {
        Task<Cart?> GetByUserIdAsync(int userId);

        Task AddItemAsync(
            int userId,
            int productId,
            int quantity);

        Task UpdateItemAsync(
            int userId,
            int productId,
            int quantity);

        Task RemoveItemAsync(
            int userId,
            int productId);

        Task ClearCartAsync(int userId);
    }
}