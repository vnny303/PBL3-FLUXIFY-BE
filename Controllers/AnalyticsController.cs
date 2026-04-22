using FluxifyAPI.Helpers;
using FluxifyAPI.Services.Common;
using FluxifyAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FluxifyAPI.Controllers
{
    [Authorize(Roles = "merchant")]
    [Route("api/tenants/{tenantId}/analytics")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview(Guid tenantId, [FromQuery] QueryTenantAnalytics query)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!TryGetPlatformUserId(out var platformUserId))
                return Unauthorized(new { message = "Token không hợp lệ hoặc thiếu userId claim" });

            var result = await _analyticsService.GetOverviewAsync(tenantId, platformUserId, query);
            return ToActionResult(result);
        }

        [HttpGet("top-products")]
        public async Task<IActionResult> GetTopProducts(Guid tenantId, [FromQuery] QueryTenantAnalytics query)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!TryGetPlatformUserId(out var platformUserId))
                return Unauthorized(new { message = "Token không hợp lệ hoặc thiếu userId claim" });

            var result = await _analyticsService.GetTopProductsAsync(tenantId, platformUserId, query);
            return ToActionResult(result);
        }

        [HttpGet("ratings")]
        public async Task<IActionResult> GetRatings(Guid tenantId, [FromQuery] QueryTenantAnalytics query)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!TryGetPlatformUserId(out var platformUserId))
                return Unauthorized(new { message = "Token không hợp lệ hoặc thiếu userId claim" });

            var result = await _analyticsService.GetRatingsAsync(tenantId, platformUserId, query);
            return ToActionResult(result);
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard(Guid tenantId, [FromQuery] QueryTenantAnalytics query)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!TryGetPlatformUserId(out var platformUserId))
                return Unauthorized(new { message = "Token không hợp lệ hoặc thiếu userId claim" });

            var result = await _analyticsService.GetDashboardAsync(tenantId, platformUserId, query);
            return ToActionResult(result);
        }

        private bool TryGetPlatformUserId(out Guid platformUserId)
        {
            var userIdClaim = User.FindFirstValue("userId");
            return Guid.TryParse(userIdClaim, out platformUserId);
        }

        private IActionResult ToActionResult<T>(ServiceResult<T> result)
        {
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }
    }
}
