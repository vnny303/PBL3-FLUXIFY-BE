using FluxifyAPI.Helpers;
using FluxifyAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FluxifyAPI.Controllers
{
    [Route("api/tenants/{tenantId}/product-skus/{productSkuId}/reviews")]
    [ApiController]
    public class ProductReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ProductReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductSkuReviews(Guid tenantId, Guid productSkuId, [FromQuery] QueryReview query)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _reviewService.GetProductSkuReviewsAsync(tenantId, productSkuId, query);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        [HttpGet("summary")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductSkuReviewSummary(Guid tenantId, Guid productSkuId)
        {
            var result = await _reviewService.GetProductSkuReviewSummaryAsync(tenantId, productSkuId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }
    }
}
