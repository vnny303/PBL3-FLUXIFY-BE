using FluxifyAPI.Data;
using FluxifyAPI.DTOs;
using FluxifyAPI.DTOs.Customer;
using FluxifyAPI.Models;
using FluxifyAPI.Services.Common;
using FluxifyAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FluxifyAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
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

        public async Task<ServiceResult<object>> RegisterMerchantAsync(RegisterMerchantRequest request)
        {
            if (await _context.PlatformUsers.AnyAsync(u => u.Email == request.Email))
                return ServiceResult<object>.Fail(400, "Email đã tồn tại!");

            if (await _context.Tenants.AnyAsync(t => t.Subdomain == request.Subdomain.ToLower()))
                return ServiceResult<object>.Fail(400, "Tên cửa hàng đã có người dùng!");

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
                new Claim("role", "merchant")
            ]);

            return ServiceResult<object>.Ok(new
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

        public async Task<ServiceResult<object>> LoginMerchantAsync(LoginRequest request)
        {
            var user = await _context.PlatformUsers
                .Include(u => u.Tenants)
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Role == "merchant");

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return ServiceResult<object>.Fail(401, "Email hoặc mật khẩu không đúng!");

            var token = GenerateToken([
                new Claim("userId", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("role", "merchant")
            ]);

            return ServiceResult<object>.Ok(new
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

        public async Task<ServiceResult<object>> RegisterCustomerAsync(string subdomain, RegisterCustomerRequest request)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Subdomain == subdomain.ToLower());
            if (tenant == null)
                return ServiceResult<object>.Fail(400, "Cửa hàng không tồn tại!");

            if (await _context.Customers.AnyAsync(c => c.TenantId == tenant.Id && c.Email == request.Email))
                return ServiceResult<object>.Fail(400, "Email đã được đăng ký!");

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

            return ServiceResult<object>.Ok(new
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

        public async Task<ServiceResult<object>> LoginCustomerAsync(string subdomain, LoginRequest request)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Subdomain == subdomain.ToLower());
            if (tenant == null)
                return ServiceResult<object>.Fail(400, "Cửa hàng không tồn tại!");

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.TenantId == tenant.Id && c.Email == request.Email);

            if (customer == null || !BCrypt.Net.BCrypt.Verify(request.Password, customer.PasswordHash))
                return ServiceResult<object>.Fail(401, "Email hoặc mật khẩu không đúng!");

            var token = GenerateToken([
                new Claim("userId", customer.Id.ToString()),
                new Claim("email", customer.Email),
                new Claim("role", "customer"),
                new Claim("tenantId", tenant.Id.ToString())
            ]);

            return ServiceResult<object>.Ok(new
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

        public async Task<ServiceResult<object>> UpdateCustomerAsync(string subdomain, string userIdFromToken, UpdateCustomerRequestDto request)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Subdomain == subdomain.ToLower());
            if (tenant == null)
                return ServiceResult<object>.Fail(400, "Cửa hàng không tồn tại!");

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id.ToString() == userIdFromToken && c.TenantId == tenant.Id);

            if (customer == null)
                return ServiceResult<object>.Fail(404, "Khách hàng không tồn tại trong hệ thống của cửa hàng này!");

            if (!BCrypt.Net.BCrypt.Verify(request.OldPass, customer.PasswordHash))
                return ServiceResult<object>.Fail(400, "Mật khẩu cũ không đúng!");

            customer.Email = request.Email;
            customer.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok(new
            {
                message = "Cập nhật thông tin thành công!",
                userId = customer.Id,
                email = customer.Email,
                role = "customer",
                tenantId = customer.TenantId
            });
        }
    }
}
