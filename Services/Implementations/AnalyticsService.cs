using FluxifyAPI.Data;
using FluxifyAPI.DTOs.Analytics;
using FluxifyAPI.Helpers;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Services.Common;
using FluxifyAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Services.Implementations
{
    public class AnalyticsService : IAnalyticsService
    {
        private static readonly HashSet<string> PaidPaymentStatuses = new(StringComparer.OrdinalIgnoreCase)
        {
            "paid",
            "completed",
            "success",
            "succeeded"
        };

        private readonly AppDbContext _context;
        private readonly ITenantRepository _tenantRepository;

        private sealed class AnalyticsOptions
        {
            public DateTime FromUtc { get; init; }
            public DateTime ToUtc { get; init; }
            public int Take { get; init; }
        }

        private sealed class OrderAggregateRecord
        {
            public Guid Id { get; init; }
            public Guid? CustomerId { get; init; }
            public string? PaymentStatus { get; init; }
            public decimal TotalAmount { get; init; }
        }

        private sealed class ProductSalesRecord
        {
            public Guid ProductId { get; init; }
            public string ProductName { get; init; } = string.Empty;
            public int QuantitySold { get; init; }
            public decimal Revenue { get; init; }
            public int OrderCount { get; init; }
        }

        private sealed class ProductRatingRecord
        {
            public Guid ProductId { get; init; }
            public decimal AverageRating { get; init; }
            public int ReviewCount { get; init; }
        }

        public AnalyticsService(AppDbContext context, ITenantRepository tenantRepository)
        {
            _context = context;
            _tenantRepository = tenantRepository;
        }

        public async Task<ServiceResult<TenantAnalyticsOverviewDto>> GetOverviewAsync(Guid tenantId, Guid platformUserId, QueryTenantAnalytics query)
        {
            var accessAndOptions = await ValidateAccessAndBuildOptionsAsync(tenantId, platformUserId, query);
            if (!accessAndOptions.Success)
                return ServiceResult<TenantAnalyticsOverviewDto>.Fail(accessAndOptions.StatusCode, accessAndOptions.Message ?? "Khong the truy xuat analytics");

            var options = accessAndOptions.Data!;
            var orderRecords = await GetOrderAggregateRecordsAsync(tenantId, options);
            var newCustomers = await CountNewCustomersAsync(tenantId, options);

            return ServiceResult<TenantAnalyticsOverviewDto>.Ok(BuildOverviewDto(options, orderRecords, newCustomers));
        }

        public async Task<ServiceResult<IEnumerable<TenantAnalyticsTopProductDto>>> GetTopProductsAsync(Guid tenantId, Guid platformUserId, QueryTenantAnalytics query)
        {
            var accessAndOptions = await ValidateAccessAndBuildOptionsAsync(tenantId, platformUserId, query);
            if (!accessAndOptions.Success)
                return ServiceResult<IEnumerable<TenantAnalyticsTopProductDto>>.Fail(accessAndOptions.StatusCode, accessAndOptions.Message ?? "Khong the truy xuat analytics");

            var options = accessAndOptions.Data!;
            var topProducts = await GetTopProductsCoreAsync(tenantId, options);

            return ServiceResult<IEnumerable<TenantAnalyticsTopProductDto>>.Ok(topProducts);
        }

        public async Task<ServiceResult<TenantAnalyticsRatingOverviewDto>> GetRatingsAsync(Guid tenantId, Guid platformUserId, QueryTenantAnalytics query)
        {
            var accessAndOptions = await ValidateAccessAndBuildOptionsAsync(tenantId, platformUserId, query);
            if (!accessAndOptions.Success)
                return ServiceResult<TenantAnalyticsRatingOverviewDto>.Fail(accessAndOptions.StatusCode, accessAndOptions.Message ?? "Khong the truy xuat analytics");

            var options = accessAndOptions.Data!;
            var ratingOverview = await GetRatingOverviewCoreAsync(tenantId, options);

            return ServiceResult<TenantAnalyticsRatingOverviewDto>.Ok(ratingOverview);
        }

        public async Task<ServiceResult<TenantAnalyticsDashboardDto>> GetDashboardAsync(Guid tenantId, Guid platformUserId, QueryTenantAnalytics query)
        {
            var accessAndOptions = await ValidateAccessAndBuildOptionsAsync(tenantId, platformUserId, query);
            if (!accessAndOptions.Success)
                return ServiceResult<TenantAnalyticsDashboardDto>.Fail(accessAndOptions.StatusCode, accessAndOptions.Message ?? "Khong the truy xuat analytics");

            var options = accessAndOptions.Data!;
            var orderRecords = await GetOrderAggregateRecordsAsync(tenantId, options);
            var newCustomers = await CountNewCustomersAsync(tenantId, options);

            var dashboard = new TenantAnalyticsDashboardDto
            {
                Overview = BuildOverviewDto(options, orderRecords, newCustomers),
                Ratings = await GetRatingOverviewCoreAsync(tenantId, options),
                TopProducts = await GetTopProductsCoreAsync(tenantId, options)
            };

            return ServiceResult<TenantAnalyticsDashboardDto>.Ok(dashboard);
        }

        private async Task<ServiceResult<AnalyticsOptions>> ValidateAccessAndBuildOptionsAsync(Guid tenantId, Guid platformUserId, QueryTenantAnalytics query)
        {
            if (!await _tenantRepository.TenantExists(tenantId))
                return ServiceResult<AnalyticsOptions>.Fail(404, "Khong tim thay tenant");

            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<AnalyticsOptions>.Forbidden("Ban khong co quyen truy cap analytics cua tenant nay");

            return BuildOptions(query);
        }

        private static ServiceResult<AnalyticsOptions> BuildOptions(QueryTenantAnalytics? query)
        {
            var normalizedQuery = query ?? new QueryTenantAnalytics();
            var toUtc = (normalizedQuery.To ?? DateTime.UtcNow).ToUniversalTime();
            var fromUtc = (normalizedQuery.From ?? toUtc.AddDays(-30)).ToUniversalTime();

            if (fromUtc > toUtc)
                return ServiceResult<AnalyticsOptions>.Fail(400, "from khong duoc lon hon to");

            return ServiceResult<AnalyticsOptions>.Ok(new AnalyticsOptions
            {
                FromUtc = fromUtc,
                ToUtc = toUtc,
                Take = normalizedQuery.Take
            });
        }

        private async Task<List<OrderAggregateRecord>> GetOrderAggregateRecordsAsync(Guid tenantId, AnalyticsOptions options)
        {
            return await _context.Orders
                .AsNoTracking()
                .Where(o => o.TenantId == tenantId
                    && o.CreatedAt.HasValue
                    && o.CreatedAt.Value >= options.FromUtc
                    && o.CreatedAt.Value <= options.ToUtc)
                .Select(o => new OrderAggregateRecord
                {
                    Id = o.Id,
                    CustomerId = o.CustomerId,
                    PaymentStatus = o.PaymentStatus,
                    TotalAmount = o.TotalAmount
                })
                .ToListAsync();
        }

        private async Task<int> CountNewCustomersAsync(Guid tenantId, AnalyticsOptions options)
        {
            return await _context.Customers
                .AsNoTracking()
                .CountAsync(c => c.TenantId == tenantId
                    && c.CreatedAt.HasValue
                    && c.CreatedAt.Value >= options.FromUtc
                    && c.CreatedAt.Value <= options.ToUtc);
        }

        private static TenantAnalyticsOverviewDto BuildOverviewDto(AnalyticsOptions options, List<OrderAggregateRecord> orderRecords, int newCustomers)
        {
            var totalOrders = orderRecords.Count;
            var grossRevenue = orderRecords.Sum(order => order.TotalAmount);
            var paidOrders = orderRecords.Count(IsPaidOrder);
            var paidRevenue = orderRecords.Where(IsPaidOrder).Sum(order => order.TotalAmount);

            var activeCustomers = orderRecords
                .Where(order => order.CustomerId.HasValue)
                .Select(order => order.CustomerId.GetValueOrDefault())
                .Distinct()
                .Count();

            return new TenantAnalyticsOverviewDto
            {
                FromUtc = options.FromUtc,
                ToUtc = options.ToUtc,
                TotalOrders = totalOrders,
                PaidOrders = paidOrders,
                GrossRevenue = grossRevenue,
                PaidRevenue = paidRevenue,
                AverageOrderValue = totalOrders == 0 ? 0 : decimal.Round(grossRevenue / totalOrders, 2, MidpointRounding.AwayFromZero),
                NewCustomers = newCustomers,
                ActiveCustomers = activeCustomers
            };
        }

        private async Task<List<TenantAnalyticsTopProductDto>> GetTopProductsCoreAsync(Guid tenantId, AnalyticsOptions options)
        {
            var topProducts = await (
                from orderItem in _context.OrderItems.AsNoTracking()
                join order in _context.Orders.AsNoTracking() on orderItem.OrderId equals order.Id
                join productSku in _context.ProductSkus.AsNoTracking() on orderItem.ProductSkuId equals productSku.Id
                join product in _context.Products.AsNoTracking() on productSku.ProductId equals product.Id
                where order.TenantId == tenantId
                    && order.CreatedAt.HasValue
                    && order.CreatedAt.Value >= options.FromUtc
                    && order.CreatedAt.Value <= options.ToUtc
                    && product.TenantId == tenantId
                group new { orderItem, order } by new { product.Id, product.Name } into grouped
                select new ProductSalesRecord
                {
                    ProductId = grouped.Key.Id,
                    ProductName = grouped.Key.Name,
                    QuantitySold = grouped.Sum(item => item.orderItem.Quantity),
                    Revenue = grouped.Sum(item => item.orderItem.UnitPrice * (decimal)item.orderItem.Quantity),
                    OrderCount = grouped.Select(item => item.order.Id).Distinct().Count()
                })
                .OrderByDescending(item => item.Revenue)
                .ThenByDescending(item => item.QuantitySold)
                .ThenBy(item => item.ProductName)
                .Take(options.Take)
                .ToListAsync();

            var productIds = topProducts.Select(item => item.ProductId).ToList();
            if (productIds.Count == 0)
                return new List<TenantAnalyticsTopProductDto>();

            var ratingsByProduct = await (
                from review in _context.Reviews.AsNoTracking()
                join productSku in _context.ProductSkus.AsNoTracking() on review.ProductSkuId equals productSku.Id
                where review.TenantId == tenantId
                    && review.CreatedAt.HasValue
                    && review.CreatedAt.Value >= options.FromUtc
                    && review.CreatedAt.Value <= options.ToUtc
                    && productIds.Contains(productSku.ProductId)
                group review by productSku.ProductId into grouped
                select new ProductRatingRecord
                {
                    ProductId = grouped.Key,
                    AverageRating = grouped.Average(item => (decimal)item.Rating),
                    ReviewCount = grouped.Count()
                })
                .ToDictionaryAsync(item => item.ProductId, item => item);

            return topProducts
                .Select(item =>
                {
                    ratingsByProduct.TryGetValue(item.ProductId, out var rating);

                    return new TenantAnalyticsTopProductDto
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        QuantitySold = item.QuantitySold,
                        Revenue = item.Revenue,
                        OrderCount = item.OrderCount,
                        AverageRating = rating == null ? 0 : decimal.Round(rating.AverageRating, 2, MidpointRounding.AwayFromZero),
                        ReviewCount = rating?.ReviewCount ?? 0
                    };
                })
                .ToList();
        }

        private async Task<TenantAnalyticsRatingOverviewDto> GetRatingOverviewCoreAsync(Guid tenantId, AnalyticsOptions options)
        {
            var ratings = await _context.Reviews
                .AsNoTracking()
                .Where(review => review.TenantId == tenantId
                    && review.CreatedAt.HasValue
                    && review.CreatedAt.Value >= options.FromUtc
                    && review.CreatedAt.Value <= options.ToUtc)
                .Select(review => review.Rating)
                .ToListAsync();

            return new TenantAnalyticsRatingOverviewDto
            {
                FromUtc = options.FromUtc,
                ToUtc = options.ToUtc,
                TotalReviews = ratings.Count,
                AverageRating = ratings.Count == 0 ? 0 : decimal.Round((decimal)ratings.Average(), 2, MidpointRounding.AwayFromZero),
                FiveStarCount = ratings.Count(rating => rating == 5),
                FourStarCount = ratings.Count(rating => rating == 4),
                ThreeStarCount = ratings.Count(rating => rating == 3),
                TwoStarCount = ratings.Count(rating => rating == 2),
                OneStarCount = ratings.Count(rating => rating == 1)
            };
        }

        private static bool IsPaidOrder(OrderAggregateRecord order)
        {
            return PaidPaymentStatuses.Contains(NormalizeDimensionKey(order.PaymentStatus));
        }

        private static string NormalizeDimensionKey(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? "unknown"
                : value.Trim().ToLowerInvariant();
        }
    }
}
