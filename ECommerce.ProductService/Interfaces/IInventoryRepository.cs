using ECommerce.ProductService.Models;

namespace ECommerce.ProductService.Interfaces
{
    public interface IInventoryRepository
    {
        Task<List<Inventory>> GetAllAsync();
        Task<Inventory?> GetByIdAsync(int id);
        Task<Inventory?> GetByProductIdAsync(int productId);
        Task AddAsync(Inventory inventory);
        Task UpdateAsync(Inventory inventory);
        Task DeleteAsync(int id);
    }
}