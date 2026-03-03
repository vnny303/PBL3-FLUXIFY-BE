using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopifyAPI.Data;
using ShopifyAPI.DTOs;
using ShopifyAPI.Models;

namespace ShopifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimpleAuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SimpleAuthController(AppDbContext context)
        {
            _context = context;
        }

        // ============================================
        // MERCHANT: ĐĂNG KÝ
        // ============================================
        [HttpPost("merchant/register")]
        public async Task<IActionResult> RegisterMerchant(RegisterMerchantRequest request)
        {
            // 1. Kiểm tra email đã tồn tại chưa
            if (await _context.PlatformUsers.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { message = "Email đã tồn tại!" });
            }

            // 2. Kiểm tra subdomain đã tồn tại chưa
            if (await _context.Tenants.AnyAsync(t => t.Subdomain == request.Subdomain))
            {
                return BadRequest(new { message = "Tên cửa hàng đã có người dùng!" });
            }

            // 3. Tạo user
            var user = new PlatformUser
            {
                Id = Guid.NewGuid(),
                Fullname = request.FullName,
                Email = request.Email,
                PasswordHash = request.Password,
                Role = "merchant",
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.PlatformUsers.Add(user);

            // 4. Tạo cửa hàng
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                OwnerId = user.Id,
                Subdomain = request.Subdomain.ToLower(),
                StoreName = request.StoreName,
                IsActive = true
            };

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            // 5. LƯU VÀO SESSION - QUAN TRỌNG!
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("Role", "merchant");
            HttpContext.Session.SetString("TenantId", tenant.Id.ToString());

            // ← THÊM DÒNG NÀY: Buộc session commit ngay lập tức
            await HttpContext.Session.CommitAsync();

            // LOG để debug
            Console.WriteLine($"=== REGISTER SUCCESS ===");
            Console.WriteLine($"UserId: {user.Id}");
            Console.WriteLine($"TenantId: {tenant.Id}");
            Console.WriteLine($"SessionId: {HttpContext.Session.Id}");

            return Ok(new
            {
                message = "Đăng ký thành công!",
                userId = user.Id,
                email = user.Email,
                role = "merchant",
                tenantId = tenant.Id,
                subdomain = tenant.Subdomain,
                sessionId = HttpContext.Session.Id // ← Trả về session ID để debug
            });
        }

        // ============================================
        // MERCHANT: ĐĂNG NHẬP
        // ============================================
        [HttpPost("merchant/login")]
        public async Task<IActionResult> LoginMerchant(LoginRequest request)
        {
            // 1. Tìm user
            var user = await _context.PlatformUsers
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Role == "merchant");

            if (user == null)
            {
                return Unauthorized(new { message = "Email không tồn tại!" });
            }

            // 2. Kiểm tra password
            if (user.PasswordHash != request.Password)
            {
                return Unauthorized(new { message = "Sai mật khẩu!" });
            }

            // 3. Lấy thông tin tenant
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.OwnerId == user.Id);

            // 4. LƯU VÀO SESSION
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("Role", "merchant");
            HttpContext.Session.SetString("TenantId", tenant?.Id.ToString() ?? "");

            // ← THÊM: Commit ngay
            await HttpContext.Session.CommitAsync();

            // LOG
            Console.WriteLine($"=== LOGIN SUCCESS ===");
            Console.WriteLine($"UserId: {user.Id}");
            Console.WriteLine($"TenantId: {tenant?.Id}");
            Console.WriteLine($"SessionId: {HttpContext.Session.Id}");

            return Ok(new
            {
                message = "Đăng nhập thành công!",
                userId = user.Id,
                email = user.Email,
                role = "merchant",
                tenantId = tenant?.Id,
                subdomain = tenant?.Subdomain,
                sessionId = HttpContext.Session.Id
            });
        }

        // ============================================
        // CUSTOMER: ĐĂNG KÝ
        // ============================================
        [HttpPost("customer/register")]
        public async Task<IActionResult> RegisterCustomer(RegisterCustomerRequest request)
        {
            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.Subdomain == request.Subdomain.ToLower());

            if (tenant == null)
            {
                return BadRequest(new { message = "Cửa hàng không tồn tại!" });
            }

            if (await _context.Customers.AnyAsync(c => c.TenantId == tenant.Id && c.Email == request.Email))
            {
                return BadRequest(new { message = "Email đã được đăng ký!" });
            }

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                Email = request.Email,
                PasswordHash = request.Password,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("UserId", customer.Id.ToString());
            HttpContext.Session.SetString("Email", customer.Email);
            HttpContext.Session.SetString("Role", "customer");
            HttpContext.Session.SetString("TenantId", tenant.Id.ToString());
            await HttpContext.Session.CommitAsync();

            return Ok(new
            {
                message = "Đăng ký thành công!",
                userId = customer.Id,
                email = customer.Email,
                role = "customer",
                tenantId = tenant.Id,
                subdomain = tenant.Subdomain,
                sessionId = HttpContext.Session.Id
            });
        }

        // ============================================
        // CUSTOMER: ĐĂNG NHẬP
        // ============================================
        [HttpPost("customer/login")]
        public async Task<IActionResult> LoginCustomer([FromQuery] string subdomain, LoginRequest request)
        {
            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.Subdomain == subdomain.ToLower());

            if (tenant == null)
            {
                return BadRequest(new { message = "Cửa hàng không tồn tại!" });
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.TenantId == tenant.Id && c.Email == request.Email);

            if (customer == null)
            {
                return Unauthorized(new { message = "Email không tồn tại!" });
            }

            if (customer.PasswordHash != request.Password)
            {
                return Unauthorized(new { message = "Sai mật khẩu!" });
            }

            HttpContext.Session.SetString("UserId", customer.Id.ToString());
            HttpContext.Session.SetString("Email", customer.Email);
            HttpContext.Session.SetString("Role", "customer");
            HttpContext.Session.SetString("TenantId", tenant.Id.ToString());
            await HttpContext.Session.CommitAsync();

            return Ok(new
            {
                message = "Đăng nhập thành công!",
                userId = customer.Id,
                email = customer.Email,
                role = "customer",
                tenantId = tenant.Id,
                subdomain = tenant.Subdomain,
                sessionId = HttpContext.Session.Id
            });
        }

        // ============================================
        // ĐĂNG XUẤT
        // ============================================
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { message = "Đăng xuất thành công!" });
        }

        // ============================================
        // XEM THÔNG TIN USER ĐANG ĐĂNG NHẬP
        // ============================================
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Chưa đăng nhập!" });
            }

            return Ok(new
            {
                userId = userId,
                email = HttpContext.Session.GetString("Email"),
                role = HttpContext.Session.GetString("Role"),
                tenantId = HttpContext.Session.GetString("TenantId"),
                sessionId = HttpContext.Session.Id
            });
        }
    }
}