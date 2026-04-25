using FluxifyAPI.DTOs.TenantPaymentSetting;
using FluxifyAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FluxifyAPI.Controllers
{
    [Authorize(Roles = "admin,merchant")]
    [Route("api/admin/tenant-payment-settings")]
    [ApiController]
    public class TenantPaymentSettingsController : ControllerBase
    {
        private readonly ITenantPaymentSettingService _service;

        public TenantPaymentSettingsController(ITenantPaymentSettingService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetByTenant([FromQuery] Guid tenantId)
        {
            if (tenantId == Guid.Empty)
                return BadRequest(new { message = "tenantId là bắt buộc" });

            if (!TryGetActor(out var actorId, out var isAdmin, out var error))
                return error!;

            var result = await _service.GetByTenantIdAsync(tenantId, actorId, isAdmin);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (!TryGetActor(out var actorId, out var isAdmin, out var error))
                return error!;

            var result = await _service.GetByIdAsync(id, actorId, isAdmin);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTenantPaymentSettingDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!TryGetActor(out var actorId, out var isAdmin, out var error))
                return error!;

            var result = await _service.CreateAsync(dto, actorId, isAdmin);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTenantPaymentSettingDto dto)
        {
            if (!TryGetActor(out var actorId, out var isAdmin, out var error))
                return error!;

            var result = await _service.UpdateAsync(id, dto, actorId, isAdmin);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!TryGetActor(out var actorId, out var isAdmin, out var error))
                return error!;

            var result = await _service.DeleteAsync(id, actorId, isAdmin);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return Ok(result.Data);
        }

        private bool TryGetActor(out Guid actorId, out bool isAdmin, out IActionResult? errorResult)
        {
            actorId = Guid.Empty;
            isAdmin = false;
            errorResult = null;

            if (!Guid.TryParse(User.FindFirstValue("userId"), out actorId))
            {
                errorResult = Unauthorized(new { message = "Token không hợp lệ hoặc thiếu userId claim" });
                return false;
            }

            var role = User.FindFirstValue("role");
            isAdmin = string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase);
            return true;
        }
    }
    /// <summary>
    /// Public endpoint để storefront/customer lấy thông tin thanh toán của tenant.
    /// Route: GET /api/tenants/{tenantId}/payment-settings
    /// Dùng khi checkout BankTransfer để hiển thị thông tin ngân hàng cho khách.
    /// </summary>
    [AllowAnonymous]
    [Route("api/tenants/{tenantId}/payment-settings")]
    [ApiController]
    public class TenantStorefrontPaymentSettingsController : ControllerBase
    {
        private readonly ITenantPaymentSettingService _service;
 
        public TenantStorefrontPaymentSettingsController(ITenantPaymentSettingService service)
        {
            _service = service;
        }
 
        // GET /api/tenants/{tenantId}/payment-settings
        // Trả về danh sách payment settings của tenant (dùng cho storefront)
        [HttpGet]
        public async Task<IActionResult> GetPaymentSettings(Guid tenantId)
        {
            // isAdmin = true để bypass quyền — đây là endpoint public read-only
            var result = await _service.GetByTenantIdAsync(tenantId, Guid.Empty, isAdmin: true);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });
 
            return Ok(result.Data);
        }
    }
}
