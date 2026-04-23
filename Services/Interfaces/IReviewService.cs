using FluxifyAPI.DTOs.Review;
using FluxifyAPI.Helpers;
using FluxifyAPI.Services.Common;

namespace FluxifyAPI.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ServiceResult<IEnumerable<ReviewDto>>> GetProductSkuReviewsAsync(Guid tenantId, Guid productSkuId, QueryReview query);
        Task<ServiceResult<ReviewSummaryDto>> GetProductSkuReviewSummaryAsync(Guid tenantId, Guid productSkuId);
        Task<ServiceResult<ReviewDto>> CreateReviewAsync(Guid tenantId, Guid customerId, CreateReviewRequestDto createDto);
        Task<ServiceResult<ReviewDto>> UpdateReviewAsync(Guid tenantId, Guid customerId, Guid reviewId, UpdateReviewRequestDto updateDto);
        Task<ServiceResult<object>> DeleteReviewAsync(Guid tenantId, Guid customerId, Guid reviewId);
    }
}
