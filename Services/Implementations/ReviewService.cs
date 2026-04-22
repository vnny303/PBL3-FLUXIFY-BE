using FluxifyAPI.DTOs.Review;
using FluxifyAPI.Helpers;
using FluxifyAPI.Mapper;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Services.Common;
using FluxifyAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductSkuRepository _productSkuRepository;
        private readonly ICustomerRepository _customerRepository;

        public ReviewService(
            IReviewRepository reviewRepository,
            IProductSkuRepository productSkuRepository,
            ICustomerRepository customerRepository)
        {
            _reviewRepository = reviewRepository;
            _productSkuRepository = productSkuRepository;
            _customerRepository = customerRepository;
        }

        public async Task<ServiceResult<IEnumerable<ReviewDto>>> GetProductSkuReviewsAsync(Guid tenantId, Guid productSkuId, QueryReview query)
        {
            query ??= new QueryReview();

            if (!await _productSkuRepository.ProductSkuExists(tenantId, productSkuId))
                return ServiceResult<IEnumerable<ReviewDto>>.Fail(404, "Không tìm thấy SKU sản phẩm");

            var reviewQuery = _reviewRepository.GetProductSkuReviewsQuery(tenantId, productSkuId);

            if (query.Rating.HasValue)
                reviewQuery = reviewQuery.Where(r => r.Rating == query.Rating.Value);

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var searchTerm = query.SearchTerm;
                reviewQuery = reviewQuery.Where(r => r.Comment.Contains(searchTerm));
            }

            var isDescending = string.Equals(query.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
            switch (query.SortBy)
            {
                case "rating":
                    reviewQuery = isDescending
                        ? reviewQuery.OrderByDescending(r => r.Rating).ThenByDescending(r => r.CreatedAt)
                        : reviewQuery.OrderBy(r => r.Rating).ThenBy(r => r.CreatedAt);
                    break;
                case "createdat":
                case "created_at":
                    reviewQuery = isDescending
                        ? reviewQuery.OrderByDescending(r => r.CreatedAt).ThenByDescending(r => r.Id)
                        : reviewQuery.OrderBy(r => r.CreatedAt).ThenBy(r => r.Id);
                    break;
                case "id":
                    reviewQuery = isDescending ? reviewQuery.OrderByDescending(r => r.Id) : reviewQuery.OrderBy(r => r.Id);
                    break;
                default:
                    reviewQuery = reviewQuery.OrderByDescending(r => r.CreatedAt).ThenByDescending(r => r.Id);
                    break;
            }

            var skipNumber = (query.Page - 1) * query.PageSize;
            var reviews = await reviewQuery.Skip(skipNumber).Take(query.PageSize).ToListAsync();
            return ServiceResult<IEnumerable<ReviewDto>>.Ok(reviews.Select(r => r.ToReviewDto()));
        }

        public async Task<ServiceResult<ReviewSummaryDto>> GetProductSkuReviewSummaryAsync(Guid tenantId, Guid productSkuId)
        {
            if (!await _productSkuRepository.ProductSkuExists(tenantId, productSkuId))
                return ServiceResult<ReviewSummaryDto>.Fail(404, "Không tìm thấy SKU sản phẩm");

            var aggregate = await _reviewRepository.GetProductSkuReviewsQuery(tenantId, productSkuId)
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Average = g.Average(r => (double?)r.Rating) ?? 0,
                    OneStar = g.Count(r => r.Rating == 1),
                    TwoStar = g.Count(r => r.Rating == 2),
                    ThreeStar = g.Count(r => r.Rating == 3),
                    FourStar = g.Count(r => r.Rating == 4),
                    FiveStar = g.Count(r => r.Rating == 5)
                })
                .FirstOrDefaultAsync();

            if (aggregate == null)
            {
                return ServiceResult<ReviewSummaryDto>.Ok(new ReviewSummaryDto
                {
                    ProductSkuId = productSkuId,
                    TotalReviews = 0,
                    AverageRating = 0,
                    OneStarCount = 0,
                    TwoStarCount = 0,
                    ThreeStarCount = 0,
                    FourStarCount = 0,
                    FiveStarCount = 0
                });
            }

            return ServiceResult<ReviewSummaryDto>.Ok(new ReviewSummaryDto
            {
                ProductSkuId = productSkuId,
                TotalReviews = aggregate.Total,
                AverageRating = decimal.Round((decimal)aggregate.Average, 2, MidpointRounding.AwayFromZero),
                OneStarCount = aggregate.OneStar,
                TwoStarCount = aggregate.TwoStar,
                ThreeStarCount = aggregate.ThreeStar,
                FourStarCount = aggregate.FourStar,
                FiveStarCount = aggregate.FiveStar
            });
        }

        public async Task<ServiceResult<ReviewDto>> CreateReviewAsync(Guid tenantId, Guid customerId, CreateReviewRequestDto createDto)
        {
            if (!await _customerRepository.CustomerExists(tenantId, customerId))
                return ServiceResult<ReviewDto>.Fail(404, "Không tìm thấy khách hàng");

            if (!await _productSkuRepository.ProductSkuExists(tenantId, createDto.ProductSkuId))
                return ServiceResult<ReviewDto>.Fail(404, "Không tìm thấy SKU sản phẩm");

            if (string.IsNullOrWhiteSpace(createDto.Comment))
                return ServiceResult<ReviewDto>.Fail(400, "Comment không được để trống");

            var existingReview = await _reviewRepository.GetCustomerProductSkuReviewAsync(tenantId, createDto.ProductSkuId, customerId);
            if (existingReview != null)
                return ServiceResult<ReviewDto>.Fail(409, "Bạn đã đánh giá SKU sản phẩm này");

            var review = createDto.ToReviewFromCreateDto(tenantId, customerId);
            var createdReview = await _reviewRepository.CreateReviewAsync(review);
            return ServiceResult<ReviewDto>.Created(createdReview.ToReviewDto());
        }

        public async Task<ServiceResult<ReviewDto>> UpdateReviewAsync(Guid tenantId, Guid customerId, Guid reviewId, UpdateReviewRequestDto updateDto)
        {
            if (!await _customerRepository.CustomerExists(tenantId, customerId))
                return ServiceResult<ReviewDto>.Fail(404, "Không tìm thấy khách hàng");

            if (string.IsNullOrWhiteSpace(updateDto.Comment))
                return ServiceResult<ReviewDto>.Fail(400, "Comment không được để trống");

            var review = await _reviewRepository.GetReviewAsync(tenantId, reviewId);
            if (review == null)
                return ServiceResult<ReviewDto>.Fail(404, "Không tìm thấy review");

            if (review.CustomerId != customerId)
                return ServiceResult<ReviewDto>.Forbidden("Bạn không có quyền cập nhật review này");

            updateDto.ToReviewFromUpdateDto(review);
            var updatedReview = await _reviewRepository.UpdateReviewAsync(review);
            return ServiceResult<ReviewDto>.Ok(updatedReview.ToReviewDto());
        }

        public async Task<ServiceResult<object>> DeleteReviewAsync(Guid tenantId, Guid customerId, Guid reviewId)
        {
            if (!await _customerRepository.CustomerExists(tenantId, customerId))
                return ServiceResult<object>.Fail(404, "Không tìm thấy khách hàng");

            var review = await _reviewRepository.GetReviewAsync(tenantId, reviewId);
            if (review == null)
                return ServiceResult<object>.Fail(404, "Không tìm thấy review");

            if (review.CustomerId != customerId)
                return ServiceResult<object>.Forbidden("Bạn không có quyền xóa review này");

            await _reviewRepository.DeleteReviewAsync(tenantId, reviewId);
            return ServiceResult<object>.Ok(new { message = "Xóa review thành công" });
        }
    }
}
