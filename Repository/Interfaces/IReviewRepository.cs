using FluxifyAPI.Models;

namespace FluxifyAPI.Repository.Interfaces
{
    public interface IReviewRepository
    {
        IQueryable<Review> GetProductSkuReviewsQuery(Guid tenantId, Guid productSkuId);
        Task<Review?> GetReviewAsync(Guid tenantId, Guid reviewId);
        Task<Review?> GetCustomerProductSkuReviewAsync(Guid tenantId, Guid productSkuId, Guid customerId);
        Task<Review> CreateReviewAsync(Review review);
        Task<Review> UpdateReviewAsync(Review review);
        Task<Review?> DeleteReviewAsync(Guid tenantId, Guid reviewId);
    }
}
