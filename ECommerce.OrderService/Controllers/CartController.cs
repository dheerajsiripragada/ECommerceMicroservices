using ECommerce.OrderService.DTOs;
using ECommerce.OrderService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.OrderService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IProductServiceClient _productServiceClient;

        public CartController(
            ICartService cartService,
            IProductServiceClient productServiceClient)
        {
            _cartService = cartService;
            _productServiceClient = productServiceClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var cart = await _cartService.GetByUserIdAsync(userId);

            if (cart == null)
                return NotFound();

            var items = new List<CartItemDto>();

            foreach (var item in cart.CartItems)
            {
                var product =
                    await _productServiceClient.GetProductAsync(
                        item.ProductId);

                if (product == null)
                    continue;

                items.Add(new CartItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = item.Quantity
                });
            }

            var cartDto = new CartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                Items = items
            };

            return Ok(cartDto);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(
            AddCartItemDto dto)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            if (dto.Quantity <= 0)
                return BadRequest(
                    "Quantity must be greater than 0.");

            try
            {
                await _cartService.AddItemAsync(
                    userId,
                    dto.ProductId,
                    dto.Quantity
                );

                return Ok("Product added to cart.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItem(
            UpdateCartItemDto dto)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            if (dto.Quantity <= 0)
                return BadRequest(
                    "Quantity must be greater than 0.");

            try
            {
                await _cartService.UpdateItemAsync(
                    userId,
                    dto.ProductId,
                    dto.Quantity
                );

                return Ok("Cart item updated.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemoveCartItem(
            int productId)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            try
            {
                await _cartService.RemoveItemAsync(
                    userId,
                    productId
                );

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            try
            {
                await _cartService.ClearCartAsync(userId);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}