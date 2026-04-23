using FluxifyAPI.DTOs.Review;
using FluxifyAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FluxifyAPI.Controllers
{
    [Authorize(Roles = "customer")]
    [Route("api/customer/reviews")]
    [ApiController]
    public class CustomerReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public CustomerReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMyReview([FromBody] CreateReviewRequestDto createDto)
        {
            if (!Guid.TryParse(User.FindFirstValue("userId"), out var customerId) ||
                !Guid.TryParse(User.FindFirstValue("tenantId"), out var tenantId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _reviewService.CreateReviewAsync(tenantId, customerId, createDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateMyReview(Guid reviewId, [FromBody] UpdateReviewRequestDto updateDto)
        {
            if (!Guid.TryParse(User.FindFirstValue("userId"), out var customerId) ||
                !Guid.TryParse(User.FindFirstValue("tenantId"), out var tenantId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _reviewService.UpdateReviewAsync(tenantId, customerId, reviewId, updateDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteMyReview(Guid reviewId)
        {
            if (!Guid.TryParse(User.FindFirstValue("userId"), out var customerId) ||
                !Guid.TryParse(User.FindFirstValue("tenantId"), out var tenantId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var result = await _reviewService.DeleteReviewAsync(tenantId, customerId, reviewId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }
    }
}
