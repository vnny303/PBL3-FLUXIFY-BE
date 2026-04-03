using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluxifyAPI.Data;
using FluxifyAPI.DTOs.Cartegory;
using FluxifyAPI.Mapper;
using FluxifyAPI.Models;

namespace FluxifyAPI.Controllers
{
    [Route("api/tenants/{tenantId}/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET
        [HttpGet]
        public async Task<ActionResult> GetCategories(Guid tenantId)
        {
            var categories = await _context.Categories
                .Where(c => c.TenantId == tenantId)
                .Select(c => new
                {
                    id = c.Id,
                    name = c.Name,
                    description = c.Description,
                    isActive = c.IsActive
                })
                .ToListAsync();

            return Ok(categories);
        }

        // POST - BỎ KIỂM TRA SESSION
        [HttpPost]
        public async Task<ActionResult> CreateCategory(Guid tenantId, [FromBody] CreateCategoryRequestDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var category = createDto.ToCategoryFromCreateDto(tenantId);

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                return Ok(category.ToCategoryDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Lỗi khi tạo danh mục",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        // PUT - BỎ KIỂM TRA SESSION
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(Guid tenantId, Guid id, [FromBody] UpdateCategoryRequestDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.Id == id);

                if (category == null)
                {
                    return NotFound(new { message = "Không tìm thấy danh mục!" });
                }

                updateDto.ToCategoryFromUpdateDto(category);

                await _context.SaveChangesAsync();

                return Ok(category.ToCategoryDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi cập nhật", error = ex.Message });
            }
        }

        // DELETE - BỎ KIỂM TRA SESSION
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid tenantId, Guid id)
        {
            try
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.Id == id);

                if (category == null)
                {
                    return NotFound();
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Xóa thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi xóa", error = ex.Message });
            }
        }
    }
}