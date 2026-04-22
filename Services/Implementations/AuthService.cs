using FluxifyAPI.DTOs;
using FluxifyAPI.DTOs.Customer;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Mapper;
using FluxifyAPI.Services.Interfaces;
using FluxifyAPI.Services.Common;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluxifyAPI.Models;

namespace FluxifyAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IPlatformUserRepository _platformUserRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IConfiguration _config;

        public AuthService(
            IPlatformUserRepository platformUserRepository,
            ITenantRepository tenantRepository,
            ICustomerRepository customerRepository,
            ICartRepository cartRepository,
            IConfiguration config)
        {
            _platformUserRepository = platformUserRepository;
            _tenantRepository = tenantRepository;
            _customerRepository = customerRepository;
            _cartRepository = cartRepository;
            _config = config;
        }

        private static string NormalizeSubdomain(string subdomain)
        {
            return subdomain.Trim().ToLowerInvariant();
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
            var normalizedSubdomain = NormalizeSubdomain(request.Subdomain);

            if (await _platformUserRepository.PlatformUserEmailExists(request.Email))
                return ServiceResult<object>.Fail(400, "Email đã tồn tại!");

            if (await _tenantRepository.GetTenantBySubdomainAsync(normalizedSubdomain) != null)
                return ServiceResult<object>.Fail(400, "Tên cửa hàng đã có người dùng!");

            var user = request.ToPlatformUserFromRegisterDto();
            await _platformUserRepository.CreatePlatformUserAsync(user);

            var tenant = request.ToTenantFromRegisterDto(user.Id);
            tenant.Subdomain = normalizedSubdomain;
            await _tenantRepository.CreateTenantAsync(tenant);

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
            var user = await _platformUserRepository.GetMerchantByEmailAsync(request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return ServiceResult<object>.Fail(401, "Email hoặc mật khẩu không đúng!");

            var firstTenantId = user.Tenants.FirstOrDefault()?.Id;

            var claims = new List<Claim>
            {
                new Claim("userId", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("role", "merchant")
            };

            var token = GenerateToken(claims);

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
            var normalizedSubdomain = NormalizeSubdomain(subdomain);
            var tenant = await _tenantRepository.GetTenantBySubdomainAsync(normalizedSubdomain);
            if (tenant == null)
                return ServiceResult<object>.Fail(400, "Cửa hàng không tồn tại!");

            if (await _customerRepository.GetCustomerByEmailAsync(tenant.Id, request.Email) != null)
                return ServiceResult<object>.Fail(400, "Email đã được đăng ký!");

            var customer = request.ToCustomerFromRegisterDto(tenant.Id);
            await _customerRepository.CreateCustomerAsync(customer);
            await _cartRepository.CreateCartAsync(new Cart
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                CustomerId = customer.Id
            });

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
            var normalizedSubdomain = NormalizeSubdomain(subdomain);
            var tenant = await _tenantRepository.GetTenantBySubdomainAsync(normalizedSubdomain);
            if (tenant == null)
                return ServiceResult<object>.Fail(400, "Cửa hàng không tồn tại!");

            var customer = await _customerRepository.GetCustomerByEmailAsync(tenant.Id, request.Email);

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

        public async Task<ServiceResult<object>> UpdateCustomerAsync(string subdomain, Guid customerId, UpdateCustomerRequestDto request)
        {
            var normalizedSubdomain = NormalizeSubdomain(subdomain);
            var tenant = await _tenantRepository.GetTenantBySubdomainAsync(normalizedSubdomain);
            if (tenant == null)
                return ServiceResult<object>.Fail(400, "Cửa hàng không tồn tại!");

            var customer = await _customerRepository.GetCustomerAsync(tenant.Id, customerId);

            if (customer == null)
                return ServiceResult<object>.Fail(404, "Khách hàng không tồn tại trong hệ thống của cửa hàng này!");

            if (string.IsNullOrWhiteSpace(request.OldPass) || !BCrypt.Net.BCrypt.Verify(request.OldPass, customer.PasswordHash))
                return ServiceResult<object>.Fail(400, "Mật khẩu cũ không đúng!");

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var normalizedEmail = request.Email.Trim();

                if (!string.Equals(normalizedEmail, customer.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var existingCustomer = await _customerRepository.GetCustomerByEmailAsync(tenant.Id, normalizedEmail);
                    if (existingCustomer != null && existingCustomer.Id != customer.Id)
                        return ServiceResult<object>.Fail(400, "Email đã được đăng ký!");
                }

                customer.Email = normalizedEmail;
            }

            if (!string.IsNullOrWhiteSpace(request.Password))
                customer.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            await _customerRepository.UpdateCustomerAsync(customer);

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




