using ECommerce.OrderService.Data;
using ECommerce.OrderService.Interfaces;
using ECommerce.OrderService.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.OrderService.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly OrderDbContext _context;

        public PaymentRepository(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Payment?> GetByOrderIdAsync(int orderId)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.OrderId == orderId);
        }

        public async Task<Payment?> GetByRazorpayOrderIdAsync(
            string razorpayOrderId)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(
                    p => p.RazorpayOrderId == razorpayOrderId);
        }

        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }
    }
}