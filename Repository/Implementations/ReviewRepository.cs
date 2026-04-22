using FluxifyAPI.Data;
using FluxifyAPI.Models;
using FluxifyAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Repository.Implementations
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Review> GetProductSkuReviewsQuery(Guid tenantId, Guid productSkuId)
        {
            return _context.Reviews
            .Where(r => r.TenantId == tenantId && r.ProductSkuId == productSkuId)
                .AsNoTracking();
        }

        public async Task<Review?> GetReviewAsync(Guid tenantId, Guid reviewId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.Id == reviewId);
        }

        public async Task<Review?> GetCustomerProductSkuReviewAsync(Guid tenantId, Guid productSkuId, Guid customerId)
        {
            return await _context.Reviews
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.ProductSkuId == productSkuId && r.CustomerId == customerId);
        }

        public async Task<Review> CreateReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review> UpdateReviewAsync(Review review)
        {
            if (_context.Entry(review).State == EntityState.Detached)
                _context.Reviews.Attach(review);

            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review?> DeleteReviewAsync(Guid tenantId, Guid reviewId)
        {
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.Id == reviewId);

            if (review == null)
                return null;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return review;
        }
    }
}
