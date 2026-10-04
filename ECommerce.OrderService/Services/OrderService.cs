using ECommerce.OrderService.Data;
using ECommerce.OrderService.Interfaces;
using ECommerce.OrderService.Models;

namespace ECommerce.OrderService.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ICartService _cartService;
        private readonly IProductServiceClient _productServiceClient;
        private readonly OrderDbContext _context;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            ICartService cartService,
            IProductServiceClient productServiceClient,
            OrderDbContext context)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _cartService = cartService;
            _productServiceClient = productServiceClient;
            _context = context;
        }

        public async Task<Order> PlaceOrderAsync(int userId)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var cart =
                    await _cartRepository.GetByUserIdAsync(userId);

                if (cart == null || !cart.CartItems.Any())
                    throw new Exception("Cart is empty");

                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    Status = "Pending",
                    TotalAmount = 0,
                    OrderItems = new List<OrderItem>()
                };

                foreach (var cartItem in cart.CartItems)
                {
                    var product =
                        await _productServiceClient
                            .GetProductAsync(cartItem.ProductId);

                    if (product == null)
                        throw new Exception(
                            $"Product not found: {cartItem.ProductId}");

                    var availableQuantity =
                        product.Quantity - product.ReservedQuantity;

                    if (cartItem.Quantity > availableQuantity)
                        throw new Exception(
                            $"Not enough stock for {product.Name}");

                    var orderItem = new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        Price = product.Price
                    };

                    order.OrderItems.Add(orderItem);

                    order.TotalAmount +=
                        product.Price * cartItem.Quantity;
                }

                foreach (var cartItem in cart.CartItems)
                {
                    await _productServiceClient
                        .ReserveInventoryAsync(
                            cartItem.ProductId,
                            cartItem.Quantity);
                }

                await _orderRepository.AddAsync(order);

                await transaction.CommitAsync();

                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _orderRepository.GetAllAsync();
        }

        public async Task UpdateStatusAsync(
            int id,
            string status)
        {
            var order =
                await _orderRepository.GetByIdAsync(id);

            if (order == null)
                throw new Exception("Order not found");

            var validStatuses = new[]
            {
                "Pending",
                "Confirmed",
                "Shipped",
                "Delivered",
                "Cancelled"
            };

            if (!validStatuses.Contains(status))
                throw new Exception("Invalid order status");

            order.Status = status;

            await _orderRepository.UpdateAsync(order);
        }

        public async Task<List<Order>> GetMyOrdersAsync(
            int userId)
        {
            return await _orderRepository
                .GetByUserIdAsync(userId);
        }
    }
}