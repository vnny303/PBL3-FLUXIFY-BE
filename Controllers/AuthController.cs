using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FluxifyAPI.Data;
using FluxifyAPI.DTOs;
using FluxifyAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluxifyAPI.DTOs.Customer;
using FluxifyAPI.Mapper;

namespace FluxifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.UtcNow.AddDays(int.Parse(_config["Jwt:ExpiryDays"]!));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ============================================
        // MERCHANT: ĐĂNG KÝ
        // ============================================
        [HttpPost("merchant/register")]
        public async Task<IActionResult> RegisterMerchant(RegisterMerchantRequest request)
        {
            if (await _context.PlatformUsers.AnyAsync(u => u.Email == request.Email))
                return BadRequest(new { message = "Email đã tồn tại!" });

            if (await _context.Tenants.AnyAsync(t => t.Subdomain == request.Subdomain.ToLower()))
                return BadRequest(new { message = "Tên cửa hàng đã có người dùng!" });

            var user = new PlatformUser
            {
                Id = Guid.NewGuid(),
                Fullname = request.FullName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "merchant",
                IsActive = true,
                CreatedAt = DateTime.Now
            };
            _context.PlatformUsers.Add(user);

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

            var token = GenerateToken([
                new Claim("userId", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("role", "merchant"),
                new Claim("tenantId", tenant.Id.ToString())
            ]);

            return Ok(new
            {
                message = "Đăng ký thành công!",
                token,
                userId = user.Id,
                email = user.Email,
                role = "merchant",
                tenantId = tenant.Id,
                subdomain = tenant.Subdomain
            });
        }

        // ============================================
        // MERCHANT: ĐĂNG NHẬP
        // ============================================
        [HttpPost("merchant/login")]
        public async Task<IActionResult> LoginMerchant(LoginRequest request)
        {
            var user = await _context.PlatformUsers
                .Include(u => u.Tenants)
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Role == "merchant");

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized(new { message = "Email hoặc mật khẩu không đúng!" });

            var token = GenerateToken([
                new Claim("userId", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("role", "merchant")
            ]);

            return Ok(new
            {
                message = "Đăng nhập thành công!",
                token,
                userId = user.Id,
                email = user.Email,
                role = "merchant",
                tenants = user.Tenants.Select(t => new
                {
                    tenantId = t.Id,
                    subdomain = t.Subdomain
                })
            });
        }

        // ============================================
        // CUSTOMER: ĐĂNG KÝ
        // ============================================
        [HttpPost("customer/register")]
        public async Task<IActionResult> RegisterCustomer([FromQuery] string subdomain, RegisterCustomerRequest request)
        {
            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.Subdomain == subdomain.ToLower());

            if (tenant == null)
                return BadRequest(new { message = "Cửa hàng không tồn tại!" });

            if (await _context.Customers.AnyAsync(c => c.TenantId == tenant.Id && c.Email == request.Email))
                return BadRequest(new { message = "Email đã được đăng ký!" });

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                IsActive = true,
                CreatedAt = DateTime.Now
            };
            _context.Customers.Add(customer);

            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                TenantId = tenant.Id,
            };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            var token = GenerateToken([
                new Claim("userId", customer.Id.ToString()),
                new Claim("email", customer.Email),
                new Claim("role", "customer"),
                new Claim("tenantId", tenant.Id.ToString())
            ]);

            return Ok(new
            {
                message = "Đăng ký thành công!",
                token,
                userId = customer.Id,
                email = customer.Email,
                role = "customer",
                tenantId = tenant.Id,
                subdomain = tenant.Subdomain
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
                return BadRequest(new { message = "Cửa hàng không tồn tại!" });

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.TenantId == tenant.Id && c.Email == request.Email);

            if (customer == null || !BCrypt.Net.BCrypt.Verify(request.Password, customer.PasswordHash))
                return Unauthorized(new { message = "Email hoặc mật khẩu không đúng!" });

            var token = GenerateToken([
                new Claim("userId", customer.Id.ToString()),
                new Claim("email", customer.Email),
                new Claim("role", "customer"),
                new Claim("tenantId", tenant.Id.ToString())
            ]);

            return Ok(new
            {
                message = "Đăng nhập thành công!",
                token,
                userId = customer.Id,
                email = customer.Email,
                role = "customer",
                tenantId = tenant.Id,
                subdomain = tenant.Subdomain
            });
        }

        // ============================================
        // XEM THÔNG TIN USER ĐANG ĐĂNG NHẬP (yêu cầu JWT)
        // ============================================
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirstValue("userId");
            var email = User.FindFirstValue("email");
            var role = User.FindFirstValue("role");
            var tenantId = User.FindFirstValue("tenantId");

            return Ok(new
            {
                userId,
                email,
                role,
                tenantId
            });
        }
        [HttpPut("customer/{customerId}")]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> UpdateCustomer([FromQuery] string subdomain, [FromRoute] string customerId, [FromBody] UpdateCustomerRequestDto request)
        {
            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.Subdomain == subdomain.ToLower());

            if (tenant == null)
                return BadRequest(new { message = "Cửa hàng không tồn tại!" });

            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
                return NotFound(new { message = "Khách hàng không tồn tại!" });
            if (!BCrypt.Net.BCrypt.Verify(request.OldPass, customer.PasswordHash))
                return BadRequest(new { message = "Mật khẩu cũ không đúng!" });
            customer.Email = request.Email;
            customer.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            await _context.SaveChangesAsync();
            return Ok(new
            {
                message = "Cập nhật thông tin thành công!",
                userId = customer.Id,
                email = customer.Email,
                role = "customer",
                tenantId = customer.TenantId
            });
        }

        // ============================================
        // ĐĂNG XUẤT
        // ============================================
        // JWT là stateless, server không lưu token nên không thể xóa được.
        // Logout thực sự là client xóa token khỏi storage.
        // Endpoint này chỉ để frontend có điểm gọi thống nhất.
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return Ok(new
            {
                caption = "Đăng xuất thành công!",
                message = "Xử lý phần này bên frontend nhé (xóa tokens)"
            });
        }
    }
}