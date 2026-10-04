using ECommerce.OrderService.Data;
using ECommerce.OrderService.Interfaces;
using ECommerce.OrderService.Models;
using Microsoft.Extensions.Options;
using Razorpay.Api;
using PaymentModel = ECommerce.OrderService.Models.Payment;
namespace ECommerce.OrderService.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductServiceClient _productServiceClient;
        private readonly OrderDbContext _context;
        private readonly RazorpaySettings _razorpaySettings;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductServiceClient productServiceClient,
            OrderDbContext context,
            IOptions<RazorpaySettings> razorpaySettings)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productServiceClient = productServiceClient;
            _context = context;
            _razorpaySettings = razorpaySettings.Value;
        }

        public async Task<PaymentModel> CreatePaymentAsync(
            int userId,
            int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
                throw new Exception("Order not found");

            if (order.UserId != userId)
                throw new Exception(
                    "You are not authorized to pay for this order");

            if (order.Status != "Pending")
                throw new Exception(
                    "Only pending orders can be paid");

            var existingPayment =
                await _paymentRepository.GetByOrderIdAsync(orderId);

            if (existingPayment != null)
            {
                if (existingPayment.Status == "Success")
                    throw new Exception(
                        "Payment already completed for this order");

                if (existingPayment.Status == "Pending")
                    return existingPayment;

                if (existingPayment.Status == "Cancelled")
                {
                    var retryClient = new RazorpayClient(
                        _razorpaySettings.KeyId,
                        _razorpaySettings.KeySecret);

                    var retryOptions = new Dictionary<string, object>
                    {
                        { "amount", (int)(order.TotalAmount * 100) },
                        { "currency", "INR" },
                        { "receipt", $"order_{order.Id}" }
                    };

                    var retryRazorpayOrder =
                        retryClient.Order.Create(retryOptions);

                    existingPayment.Status = "Pending";
                    existingPayment.RazorpayOrderId =
                        retryRazorpayOrder["id"].ToString();
                    existingPayment.RazorpayPaymentId = null;
                    existingPayment.PaymentDate = DateTime.UtcNow;

                    await _paymentRepository.UpdateAsync(existingPayment);

                    return existingPayment;
                }
            }

            var client = new RazorpayClient(
                _razorpaySettings.KeyId,
                _razorpaySettings.KeySecret);

            var options = new Dictionary<string, object>
            {
                { "amount", (int)(order.TotalAmount * 100) },
                { "currency", "INR" },
                { "receipt", $"order_{order.Id}" }
            };

            var razorpayOrder = client.Order.Create(options);

            var payment = new PaymentModel
            {
                OrderId = order.Id,
                Amount = order.TotalAmount,
                Status = "Pending",
                PaymentMethod = "Razorpay",
                RazorpayOrderId = razorpayOrder["id"].ToString(),
                PaymentDate = DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment);

            return payment;
        }

        public async Task<PaymentModel?> GetByOrderIdAsync(int orderId)
        {
            return await _paymentRepository
                .GetByOrderIdAsync(orderId);
        }

        public async Task<bool> VerifyPaymentAsync(
            int userId,
            string razorpayOrderId,
            string razorpayPaymentId,
            string razorpaySignature)
        {
            var payment =
                await _paymentRepository
                    .GetByRazorpayOrderIdAsync(razorpayOrderId);

            if (payment == null)
                throw new Exception("Payment not found");

            var order =
                await _orderRepository.GetByIdAsync(payment.OrderId);

            if (order == null)
                throw new Exception("Order not found");

            if (order.UserId != userId)
                throw new Exception(
                    "You are not authorized to verify this payment");

            if (order.Status != "Pending")
                throw new Exception(
                    "Order is no longer pending");

            if (payment.Status == "Success")
                return true;

            var attributes = new Dictionary<string, string>
            {
                { "razorpay_order_id", razorpayOrderId },
                { "razorpay_payment_id", razorpayPaymentId },
                { "razorpay_signature", razorpaySignature }
            };

            try
            {
                Utils.verifyPaymentSignature(attributes);
            }
            catch
            {
                payment.Status = "Failed";
                payment.RazorpayPaymentId = razorpayPaymentId;

                await _paymentRepository.UpdateAsync(payment);

                return false;
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var cart =
                    await _cartRepository.GetByUserIdAsync(userId);

                if (cart == null || !cart.CartItems.Any())
                    throw new Exception("Cart is empty");

                foreach (var orderItem in order.OrderItems)
                {
                    var product =
                        await _productServiceClient
                            .GetProductAsync(orderItem.ProductId);

                    if (product == null)
                        throw new Exception(
                            $"Product not found for product {orderItem.ProductId}");

                    if (orderItem.Quantity > product.Quantity)
                        throw new Exception(
                            $"Not enough stock for {product.Name}");

                    if (product.ReservedQuantity < orderItem.Quantity)
                        throw new Exception(
                            $"Reserved stock is insufficient for {product.Name}");
                }

                foreach (var orderItem in order.OrderItems)
                {
                    await _productServiceClient
                        .ConfirmInventoryAsync(
                            orderItem.ProductId,
                            orderItem.Quantity);
                }

                foreach (var orderItem in order.OrderItems)
                {
                    var cartItem = cart.CartItems
                        .FirstOrDefault(
                            item => item.ProductId == orderItem.ProductId);

                    if (cartItem == null)
                        continue;

                    if (cartItem.Quantity <= orderItem.Quantity)
                    {
                        cart.CartItems.Remove(cartItem);
                    }
                    else
                    {
                        cartItem.Quantity -= orderItem.Quantity;
                    }
                }

                payment.Status = "Success";
                payment.RazorpayPaymentId = razorpayPaymentId;
                payment.PaymentDate = DateTime.UtcNow;

                order.Status = "Confirmed";

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task CancelPaymentAsync(
            int userId,
            int orderId)
        {
            var payment =
                await _paymentRepository.GetByOrderIdAsync(orderId);

            if (payment == null)
                throw new Exception("Payment not found");

            var order =
                await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
                throw new Exception("Order not found");

            if (order.UserId != userId)
                throw new Exception(
                    "You are not authorized to cancel this payment");

            if (order.Status != "Pending")
                throw new Exception(
                    "Only pending orders can be cancelled");

            if (payment.Status != "Pending")
                throw new Exception(
                    "Payment is no longer pending");

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var orderItem in order.OrderItems)
                {
                    var product =
                        await _productServiceClient
                            .GetProductAsync(orderItem.ProductId);

                    if (product == null)
                        throw new Exception(
                            $"Product not found for product {orderItem.ProductId}");

                    if (product.ReservedQuantity < orderItem.Quantity)
                        throw new Exception(
                            $"Reserved stock is insufficient for {product.Name}");

                    await _productServiceClient
                        .ReleaseInventoryAsync(
                            orderItem.ProductId,
                            orderItem.Quantity);
                }

                payment.Status = "Cancelled";

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}