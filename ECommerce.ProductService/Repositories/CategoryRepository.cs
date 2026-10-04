using ECommerce.ProductService.Data;
using ECommerce.ProductService.Interfaces;
using ECommerce.ProductService.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.ProductService.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ProductDbContext _context;

        public CategoryRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Category>> GetByIdsAsync(
            IEnumerable<int> ids)
        {
            return await _context.Categories
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}