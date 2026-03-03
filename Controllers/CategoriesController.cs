using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopifyAPI.Data;
using ShopifyAPI.Models;
using System.Text.Json;

namespace ShopifyAPI.Controllers
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
        public async Task<ActionResult> CreateCategory(Guid tenantId, [FromBody] JsonElement data)
        {
            try
            {
                Console.WriteLine("=== CREATE CATEGORY ===");
                Console.WriteLine($"TenantId: {tenantId}");
                Console.WriteLine($"Raw Data: {data}");

                // Parse JSON
                string name = data.GetProperty("name").GetString() ?? "";
                string? description = data.TryGetProperty("description", out var descProp)
                    ? descProp.GetString()
                    : null;
                bool isActive = data.TryGetProperty("isActive", out var activeProp)
                    ? activeProp.GetBoolean()
                    : true;

                Console.WriteLine($"Parsed - Name: {name}, Description: {description}, IsActive: {isActive}");

                // Validate
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest(new { message = "Tên danh mục không được để trống!" });
                }

                // Create
                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = name.Trim(),
                    Description = description?.Trim(),
                    IsActive = isActive
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ Category created: {category.Id}");

                return Ok(new
                {
                    id = category.Id,
                    name = category.Name,
                    description = category.Description,
                    isActive = category.IsActive
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                Console.WriteLine($"InnerException: {ex.InnerException?.Message}");

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
        public async Task<IActionResult> UpdateCategory(Guid tenantId, Guid id, [FromBody] JsonElement data)
        {
            try
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.Id == id);

                if (category == null)
                {
                    return NotFound(new { message = "Không tìm thấy danh mục!" });
                }

                category.Name = data.GetProperty("name").GetString() ?? category.Name;
                category.Description = data.TryGetProperty("description", out var descProp)
                    ? descProp.GetString()
                    : category.Description;
                category.IsActive = data.TryGetProperty("isActive", out var activeProp)
                    ? activeProp.GetBoolean()
                    : category.IsActive;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    id = category.Id,
                    name = category.Name,
                    description = category.Description,
                    isActive = category.IsActive
                });
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