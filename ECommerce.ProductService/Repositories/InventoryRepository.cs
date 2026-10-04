using ECommerce.ProductService.Data;
using ECommerce.ProductService.Interfaces;
using ECommerce.ProductService.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.ProductService.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ProductDbContext _context;

        public InventoryRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<List<Inventory>> GetAllAsync()
        {
            return await _context.Inventories
                .ToListAsync();
        }

        public async Task<Inventory?> GetByIdAsync(int id)
        {
            return await _context.Inventories
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Inventory?> GetByProductIdAsync(int productId)
        {
            return await _context.Inventories
                .FirstOrDefaultAsync(i => i.ProductId == productId);
        }

        public async Task AddAsync(Inventory inventory)
        {
            await _context.Inventories.AddAsync(inventory);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Inventory inventory)
        {
            _context.Inventories.Update(inventory);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory != null)
            {
                _context.Inventories.Remove(inventory);
                await _context.SaveChangesAsync();
            }
        }
    }
}