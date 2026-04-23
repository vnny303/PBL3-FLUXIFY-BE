using FluxifyAPI.DTOs.Review;
using FluxifyAPI.Models;

namespace FluxifyAPI.Mapper
{
    public static class ReviewMapper
    {
        public static ReviewDto ToReviewDto(this Review review)
        {
            return new ReviewDto
            {
                Id = review.Id,
                TenantId = review.TenantId,
                ProductSkuId = review.ProductSkuId,
                CustomerId = review.CustomerId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt
            };
        }

        public static Review ToReviewFromCreateDto(this CreateReviewRequestDto createDto, Guid tenantId, Guid customerId)
        {
            var now = DateTime.UtcNow;
            return new Review
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ProductSkuId = createDto.ProductSkuId,
                CustomerId = customerId,
                Rating = createDto.Rating,
                Comment = createDto.Comment.Trim(),
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        public static Review ToReviewFromUpdateDto(this UpdateReviewRequestDto updateDto, Review existingReview)
        {
            existingReview.Rating = updateDto.Rating;
            existingReview.Comment = updateDto.Comment.Trim();
            existingReview.UpdatedAt = DateTime.UtcNow;
            return existingReview;
        }
    }
}
