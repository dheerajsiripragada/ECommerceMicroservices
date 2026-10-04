using ECommerce.ProductService.DTOs;
using ECommerce.ProductService.Interfaces;
using ECommerce.ProductService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.ProductService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("get-all-products")]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();

            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? string.Empty,
                Quantity = p.Inventory?.Quantity ?? 0,
                ReservedQuantity = p.Inventory?.ReservedQuantity ?? 0
            }).ToList();

            return Ok(productDtos);
        }
        [HttpGet("get-products-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? string.Empty,
                Quantity = product.Inventory?.Quantity ?? 0,
                ReservedQuantity = product.Inventory?.ReservedQuantity ?? 0
            };

            return Ok(productDto);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("Add-Products")]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId
            };

            await _productService.AddAsync(product);

            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId
            };

            return Ok(productDto);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("Update-product-by-id/{id}")]
        public async Task<IActionResult> Update(
    int id,
    UpdateProductDto dto)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.ImageUrl = dto.ImageUrl;
            product.CategoryId = dto.CategoryId;

            await _productService.UpdateAsync(product);

            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId
            };

            return Ok(productDto);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete-product-by-id/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            await _productService.DeleteAsync(id);

            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("import-products")]
        public async Task<IActionResult> ImportProducts(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please upload an Excel file.");
            }

            if (!Path.GetExtension(file.FileName)
                .Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only .xlsx files are supported.");
            }

            try
            {
                using var stream = file.OpenReadStream();

                await _productService.ImportProductsAsync(stream);

                return Ok("Products imported successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (FormatException ex)
            {
                return BadRequest($"Invalid Excel data: {ex.Message}");
            }
        }
    }
}