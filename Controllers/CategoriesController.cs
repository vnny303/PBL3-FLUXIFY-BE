using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FluxifyAPI.DTOs.Cartegory;
using FluxifyAPI.Helpers;
using FluxifyAPI.Services.Interfaces;
using System.Security.Claims;

namespace FluxifyAPI.Controllers
{
    [Authorize(Roles = "merchant")]
    [Route("api/tenants/{tenantId}/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetCategories([FromRoute] Guid tenantId, [FromQuery] QueryCategory query)
        {
            var result = await _categoryService.GetCategoriesAsync(tenantId, query);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });
            return StatusCode(result.StatusCode, result.Data);
        }
        [HttpGet("{categoryId}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetCategoriesById([FromRoute] Guid tenantId, [FromRoute] Guid categoryId)
        {
            var result = await _categoryService.GetCategoryByIdAsync(tenantId, categoryId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });
            return StatusCode(result.StatusCode, result.Data);
        }

        [HttpPost]
        public async Task<ActionResult> CreateCategory(Guid tenantId, [FromBody] CreateCategoryRequestDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!Guid.TryParse(User.FindFirstValue("userId"), out var userId))
                return Unauthorized(new { message = "Token không hợp lệ hoặc thiếu userId claim" });
            var result = await _categoryService.CreateCategoryAsync(tenantId, userId, createDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });
            return StatusCode(result.StatusCode, result.Data);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(Guid tenantId, Guid id, [FromBody] UpdateCategoryRequestDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!Guid.TryParse(User.FindFirstValue("userId"), out var userId))
                return Unauthorized(new { message = "Token không hợp lệ hoặc thiếu userId claim" });
            var result = await _categoryService.UpdateCategoryAsync(tenantId, userId, id, updateDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });
            return StatusCode(result.StatusCode, result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid tenantId, Guid id)
        {
            if (!Guid.TryParse(User.FindFirstValue("userId"), out var userId))
                return Unauthorized(new { message = "Token không hợp lệ hoặc thiếu userId claim" });
            var result = await _categoryService.DeleteCategoryAsync(tenantId, userId, id);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });
            return StatusCode(result.StatusCode, result.Data);
        }
    }
}

