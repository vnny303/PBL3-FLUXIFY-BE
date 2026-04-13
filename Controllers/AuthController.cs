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
        // MERCHANT: ÐANG KÝ
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
        // MERCHANT: ÐANG NH?P
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
        // CUSTOMER: ÐANG KÝ
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
        // CUSTOMER: ÐANG NH?P
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
        // XEM THÔNG TIN USER ÐANG ÐANG NH?P (yêu c?u JWT)
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
                return Unauthorized(new { message = "Token không h?p l?" });

            if (role != "customer" && role != "merchant")
                return Unauthorized(new { message = "Role không h?p l? trong token" });
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
                return Unauthorized(new { message = "Token không h?p l?" });

            if (customerIdFromToken != customerId)
                return StatusCode(403, new { message = "B?n không có quy?n c?p nh?t tài kho?n này" });

            var result = await _authService.UpdateCustomerAsync(subdomain, customerIdFromToken, request);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // ============================================
        // ÐANG XU?T
        // ============================================
        // JWT là stateless, server không luu token nên không th? xóa du?c.
        // Logout th?c s? là client xóa token kh?i storage.
        // Endpoint này ch? d? frontend có di?m g?i th?ng nh?t.
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return Ok(new
            {
                caption = "Ðang xu?t thành công!",
                message = "X? lý ph?n này bên frontend nhé (xóa tokens)"
            });
        }
    }
}

