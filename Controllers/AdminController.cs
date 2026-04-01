using FluxifyAPI.Data;
using FluxifyAPI.Models;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Controllers
{
    // Class này sẽ chứa các API dành cho admin, như quản lý người dùng, quản lý sản phẩm, v.v.
    [Route("api/[controller]")]

    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("platformUsers")]
        public async Task<List<PlatformUser>> GetAllPlatformUsers()
        {
            return await _context.PlatformUsers
                .AsSplitQuery()
                .Include(p => p.Tenants)
                    .ThenInclude(t => t.Categories)
                        .ThenInclude(c => c.Products)
                            .ThenInclude(p => p.ProductSkus)
                .Include(p => p.Tenants)
                    .ThenInclude(t => t.Customers)
                        .ThenInclude(c => c.Cart)
                            .ThenInclude(cart => cart.CartItems)
                .Include(p => p.Tenants)
                    .ThenInclude(t => t.Customers)
                        .ThenInclude(c => c.Orders)
                            .ThenInclude(o => o.OrderItems)
                .ToListAsync();
        }
        [HttpDelete("platformUsers/{id}")]
        public async Task<IActionResult> DeletePlatformUser(Guid id)
        {
            var platformUser = await _context.PlatformUsers.FindAsync(id);
            if (platformUser == null)
                return NotFound("Id người dùng không hợp lệ");

            _context.PlatformUsers.Remove(platformUser);
            await _context.SaveChangesAsync();

            return Ok(platformUser);
        }
    }
}