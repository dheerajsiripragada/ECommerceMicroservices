using ECommerce.ProductService.Models;

namespace ECommerce.ProductService.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<List<Product>> GetByIdsAsync(List<int> ids);
        Task SaveBatchAsync(List<Product> productsToAdd,List<Product> productsToUpdate);
    }
}