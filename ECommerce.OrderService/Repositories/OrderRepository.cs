using ECommerce.OrderService.Data;
using ECommerce.OrderService.Interfaces;
using ECommerce.OrderService.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.OrderService.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;

        public OrderRepository(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Payment)
                .ToListAsync();
        }

        public async Task<List<Order>> GetByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Payment)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}