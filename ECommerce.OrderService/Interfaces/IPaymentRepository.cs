using ECommerce.OrderService.Models;

namespace ECommerce.OrderService.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(int id);
        Task<Payment?> GetByOrderIdAsync(int orderId);
        Task<Payment?> GetByRazorpayOrderIdAsync(string razorpayOrderId);
        Task AddAsync(Payment payment);
        Task UpdateAsync(Payment payment);
    }
}