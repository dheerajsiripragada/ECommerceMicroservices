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
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IProductServiceClient _productServiceClient;

        public OrderController(
            IOrderService orderService,
            IProductServiceClient productServiceClient)
        {
            _orderService = orderService;
            _productServiceClient = productServiceClient;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            try
            {
                var order = await _orderService.PlaceOrderAsync(userId);

                var orderDto = await MapToDtoAsync(order);

                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var orders = await _orderService.GetMyOrdersAsync(userId);

            var orderDtos = new List<OrderDto>();

            foreach (var order in orders)
            {
                orderDtos.Add(await MapToDtoAsync(order));
            }

            return Ok(orderDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var currentUserId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var order = await _orderService.GetByIdAsync(id);

            if (order == null)
                return NotFound();

            if (currentUserRole != "Admin" &&
                order.UserId != currentUserId)
            {
                return Forbid();
            }

            var orderDto = await MapToDtoAsync(order);

            return Ok(orderDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllAsync();

            var orderDtos = new List<OrderDto>();

            foreach (var order in orders)
            {
                orderDtos.Add(await MapToDtoAsync(order));
            }

            return Ok(orderDtos);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            UpdateOrderStatusDto dto)
        {
            try
            {
                await _orderService.UpdateStatusAsync(id, dto.Status);

                return Ok("Order status updated.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<OrderDto> MapToDtoAsync(
            ECommerce.OrderService.Models.Order order)
        {
            var items = new List<OrderItemDto>();

            foreach (var item in order.OrderItems)
            {
                var product =
                    await _productServiceClient.GetProductAsync(item.ProductId);

                items.Add(new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = product?.Name ?? "Unknown Product",
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Items = items
            };
        }
    }
}