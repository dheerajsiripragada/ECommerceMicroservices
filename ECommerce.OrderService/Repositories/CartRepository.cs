using ECommerce.OrderService.Data;
using ECommerce.OrderService.Interfaces;
using ECommerce.OrderService.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.OrderService.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly OrderDbContext _context;

        public CartRepository(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task AddAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cart cart)
        {
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var cart = await _context.Carts.FindAsync(id);

            if (cart != null)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }
    }
}