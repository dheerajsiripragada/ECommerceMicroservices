using ECommerce.ProductService.DTOs;
using ECommerce.ProductService.Interfaces;
using ECommerce.ProductService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.ProductService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-all-inventory")]
        public async Task<IActionResult> GetAll()
        {
            var inventories = await _inventoryService.GetAllAsync();

            var inventoryDtos = inventories.Select(i => new InventoryDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList();

            return Ok(inventoryDtos);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-by-product/{productId}")]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            var inventory =
                await _inventoryService.GetByProductIdAsync(productId);

            if (inventory == null)
            {
                return NotFound();
            }

            var inventoryDto = new InventoryDto
            {
                Id = inventory.Id,
                ProductId = inventory.ProductId,
                Quantity = inventory.Quantity
            };

            return Ok(inventoryDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add-inventory")]
        public async Task<IActionResult> Create(InventoryDto dto)
        {
            var inventory = new Inventory
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };

            await _inventoryService.AddAsync(inventory);

            var inventoryDto = new InventoryDto
            {
                Id = inventory.Id,
                ProductId = inventory.ProductId,
                Quantity = inventory.Quantity
            };

            return Ok(inventoryDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-inventory/{id}")]
        public async Task<IActionResult> Update(
            int id,
            InventoryDto dto)
        {
            var inventory =
                await _inventoryService.GetByIdAsync(id);

            if (inventory == null)
            {
                return NotFound();
            }

            inventory.Quantity = dto.Quantity;

            await _inventoryService.UpdateAsync(inventory);

            var inventoryDto = new InventoryDto
            {
                Id = inventory.Id,
                ProductId = inventory.ProductId,
                Quantity = inventory.Quantity
            };

            return Ok(inventoryDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-inventory/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var inventory =
                await _inventoryService.GetByIdAsync(id);

            if (inventory == null)
            {
                return NotFound();
            }

            await _inventoryService.DeleteAsync(id);

            return NoContent();
        }
        [Authorize]
        [HttpPost("reserve")]
        public async Task<IActionResult> Reserve(ReserveInventoryDto dto)
        {
            try
            {
                await _inventoryService.ReserveAsync(
                    dto.ProductId,
                    dto.Quantity);

                return Ok("Inventory reserved successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmInventory(
    [FromBody] ReserveInventoryDto dto)
        {
            try
            {
                await _inventoryService.ConfirmInventoryAsync(
                    dto.ProductId,
                    dto.Quantity);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("release")]
        public async Task<IActionResult> ReleaseInventory(
            [FromBody] ReserveInventoryDto dto)
        {
            try
            {
                await _inventoryService.ReleaseInventoryAsync(
                    dto.ProductId,
                    dto.Quantity);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}