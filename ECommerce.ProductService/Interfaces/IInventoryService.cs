using ECommerce.ProductService.Models;

namespace ECommerce.ProductService.Interfaces
{
    public interface IInventoryService
    {
        Task<List<Inventory>> GetAllAsync();
        Task<Inventory?> GetByIdAsync(int id);
        Task<Inventory?> GetByProductIdAsync(int productId);
        Task AddAsync(Inventory inventory);
        Task UpdateAsync(Inventory inventory);
        Task DeleteAsync(int id);
        Task ReserveAsync(int productId, int quantity);
        Task ConfirmInventoryAsync(int productId, int quantity);
        Task ReleaseInventoryAsync(int productId, int quantity);
    }
}