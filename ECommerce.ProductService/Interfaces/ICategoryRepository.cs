using ECommerce.ProductService.Models;

namespace ECommerce.ProductService.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();

        Task<Category?> GetByIdAsync(int id);

        Task<List<Category>> GetByIdsAsync(IEnumerable<int> ids);

        Task AddAsync(Category category);

        Task UpdateAsync(Category category);

        Task DeleteAsync(int id);
    }
}