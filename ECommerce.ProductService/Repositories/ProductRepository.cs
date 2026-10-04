using ECommerce.ProductService.Data;
using ECommerce.ProductService.Interfaces;
using ECommerce.ProductService.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.ProductService.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _context;

        public ProductRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await GetByIdAsync(id);

            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Product>> GetByIdsAsync(List<int> ids)
        {
            return await _context.Products
                .Include(p => p.Inventory)
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();
        }
        public async Task SaveBatchAsync(List<Product> productsToAdd,List<Product> productsToUpdate)
        {
            if (productsToAdd.Any())
            {
                await _context.Products.AddRangeAsync(productsToAdd);
            }

            if (productsToUpdate.Any())
            {
                _context.Products.UpdateRange(productsToUpdate);
            }

            await _context.SaveChangesAsync();
        }
    }
}