using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FluxifyAPI.DTOs.Cartegory;
using FluxifyAPI.Helpers;
using FluxifyAPI.Services.Interfaces;

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
        public async Task<ActionResult> GetCategories(Guid tenantId, [FromQuery] QueryCategory query)
        {
            var result = await _categoryService.GetCategoriesAsync(tenantId, query);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // POST - B? KI?M TRA SESSION
        [HttpPost]
        public async Task<ActionResult> CreateCategory(Guid tenantId, [FromBody] CreateCategoryRequestDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryService.CreateCategoryAsync(tenantId, createDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // PUT - B? KI?M TRA SESSION
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(Guid tenantId, Guid id, [FromBody] UpdateCategoryRequestDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryService.UpdateCategoryAsync(tenantId, id, updateDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // DELETE - B? KI?M TRA SESSION
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid tenantId, Guid id)
        {
            var result = await _categoryService.DeleteCategoryAsync(tenantId, id);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }
    }
}

