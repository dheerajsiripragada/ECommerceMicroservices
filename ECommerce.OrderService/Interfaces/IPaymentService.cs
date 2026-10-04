using ECommerce.OrderService.Models;

namespace ECommerce.OrderService.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment> CreatePaymentAsync(int userId, int orderId);
        Task<Payment?> GetByOrderIdAsync(int orderId);
        Task<bool> VerifyPaymentAsync(int userId,string razorpayOrderId,string razorpayPaymentId,string razorpaySignature);
        Task CancelPaymentAsync(int userId, int orderId);
    }
}