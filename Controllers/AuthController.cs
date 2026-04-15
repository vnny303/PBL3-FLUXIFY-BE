using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluxifyAPI.DTOs;
using System.Security.Claims;
using FluxifyAPI.DTOs.Customer;
using FluxifyAPI.Services.Interfaces;

namespace FluxifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // ============================================
        // MERCHANT: ĐĂNG KÝ
        // ============================================
        [HttpPost("merchant/register")]
        public async Task<IActionResult> RegisterMerchant(RegisterMerchantRequest request)
        {
            var result = await _authService.RegisterMerchantAsync(request);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });
            return StatusCode(result.StatusCode, result.Data);
        }

        // ============================================
        // MERCHANT: ĐĂNG NHẬP
        // ============================================
        [HttpPost("merchant/login")]
        public async Task<IActionResult> LoginMerchant(LoginRequest request)
        {
            var result = await _authService.LoginMerchantAsync(request);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });
            return StatusCode(result.StatusCode, result.Data);
        }

        // ============================================
        // CUSTOMER: ĐĂNG KÝ
        // ============================================
        [HttpPost("customer/register")]
        public async Task<IActionResult> RegisterCustomer([FromQuery] string subdomain, RegisterCustomerRequest request)
        {
            var result = await _authService.RegisterCustomerAsync(subdomain, request);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });
            return StatusCode(result.StatusCode, result.Data);
        }

        // ============================================
        // CUSTOMER: ĐĂNG NHẬP
        // ============================================
        [HttpPost("customer/login")]
        public async Task<IActionResult> LoginCustomer([FromQuery] string subdomain, LoginRequest request)
        {
            var result = await _authService.LoginCustomerAsync(subdomain, request);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });
            return StatusCode(result.StatusCode, result.Data);
        }

        // ============================================
        // XEM THÔNG TIN USER ĐANG ĐĂNG NHẬP (yêu cầu JWT)
        // ============================================
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst("userId")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst("email")?.Value
                ?? User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst("role")?.Value
                ?? User.FindFirst(ClaimTypes.Role)?.Value;
            var tenantId = User.FindFirst("tenantId")?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role))
                return Unauthorized(new { message = "Token không hợp lệ" });

            if (role != "customer" && role != "merchant")
                return Unauthorized(new { message = "Role không hợp lệ trong token" });
            if (role == "customer")
                return Ok(new
                {
                    userId,
                    email,
                    role,
                    tenantId
                });
            else // merchant
                return Ok(new
                {
                    userId,
                    email,
                    role
                });
        }
        [HttpPut("customer/{customerId}")]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> UpdateCustomer(Guid customerId, [FromQuery] string subdomain, [FromBody] UpdateCustomerRequestDto request)
        {
            var userIdFromToken = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdFromToken) || !Guid.TryParse(userIdFromToken, out var customerIdFromToken))
                return Unauthorized(new { message = "Token không hợp lệ" });

            if (customerIdFromToken != customerId)
                return StatusCode(403, new { message = "Bạn không có quyền cập nhật tài khoản này" });

            var result = await _authService.UpdateCustomerAsync(subdomain, customerIdFromToken, request);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
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