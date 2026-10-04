using ECommerce.OrderService.Interfaces;
using ECommerce.OrderService.Models;

namespace ECommerce.OrderService.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductServiceClient _productServiceClient;

        public CartService(
            ICartRepository cartRepository,
            IProductServiceClient productServiceClient)
        {
            _cartRepository = cartRepository;
            _productServiceClient = productServiceClient;
        }

        public async Task<Cart?> GetByUserIdAsync(int userId)
        {
            return await _cartRepository.GetByUserIdAsync(userId);
        }

        public async Task AddItemAsync(
            int userId,
            int productId,
            int quantity)
        {
            var product =
                await _productServiceClient.GetProductAsync(productId);

            if (product == null)
                throw new Exception("Product not found");

            if (quantity > product.Quantity)
                throw new Exception("Not enough stock available");

            var cart =
                await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };

                cart.CartItems.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity
                });

                await _cartRepository.AddAsync(cart);

                return;
            }

            var existingItem = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == productId);

            if (existingItem != null)
            {
                var newQuantity =
                    existingItem.Quantity + quantity;

                if (newQuantity > product.Quantity)
                    throw new Exception("Not enough stock available");

                existingItem.Quantity = newQuantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            await _cartRepository.UpdateAsync(cart);
        }

        public async Task UpdateItemAsync(
            int userId,
            int productId,
            int quantity)
        {
            var cart =
                await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
                throw new Exception("Cart not found");

            var item = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == productId);

            if (item == null)
                throw new Exception("Product not found in cart");

            var product =
                await _productServiceClient.GetProductAsync(productId);

            if (product == null)
                throw new Exception("Product not found");

            if (quantity > product.Quantity)
                throw new Exception("Not enough stock available");

            item.Quantity = quantity;

            await _cartRepository.UpdateAsync(cart);
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart =
                await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
                throw new Exception("Cart not found");

            cart.CartItems.Clear();

            await _cartRepository.UpdateAsync(cart);
        }

        public async Task RemoveItemAsync(
            int userId,
            int productId)
        {
            var cart =
                await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
                throw new Exception("Cart not found");

            var item = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == productId);

            if (item == null)
                throw new Exception("Product not found in cart");

            cart.CartItems.Remove(item);

            await _cartRepository.UpdateAsync(cart);
        }
    }
}