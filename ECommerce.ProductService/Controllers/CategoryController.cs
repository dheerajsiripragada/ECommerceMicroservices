using ECommerce.ProductService.DTOs;
using ECommerce.ProductService.Interfaces;
using ECommerce.ProductService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.ProductService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("get-all-categories")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();

            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            return Ok(categoryDtos);
        }

        [HttpGet("get-category-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };

            return Ok(categoryDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add-category")]
        public async Task<IActionResult> Create(CategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name
            };

            await _categoryService.AddAsync(category);

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };

            return Ok(categoryDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-category/{id}")]
        public async Task<IActionResult> Update(
            int id,
            CategoryDto dto)
        {
            var category = await _categoryService.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            category.Name = dto.Name;

            await _categoryService.UpdateAsync(category);

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };

            return Ok(categoryDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-category/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            await _categoryService.DeleteAsync(id);

            return NoContent();
        }
    }
}