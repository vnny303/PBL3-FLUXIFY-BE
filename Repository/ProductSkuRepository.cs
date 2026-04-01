using FluxifyAPI.Data;
using FluxifyAPI.Interfaces;
using FluxifyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Repository
{
    public class ProductSkuRepository : IProductSkuRepository
    {
        private readonly AppDbContext _context;

        public ProductSkuRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductSku?> GetProductSkuAsync(Guid tenantId, Guid productSkuId)
        {
            return await _context.ProductSkus
                .Include(ps => ps.Product)
                .FirstOrDefaultAsync(ps => ps.Id == productSkuId && ps.Product.TenantId == tenantId);
        }

        public async Task<List<ProductSku>?> GetProductSkusByProductAsync(Guid tenantId, Guid productId)
        {
            return await _context.ProductSkus
                .Include(ps => ps.Product)
                .Where(ps => ps.ProductId == productId && ps.Product.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<ProductSku> CreateProductSkuAsync(Guid tenantId, Guid productId, string skuCode, decimal price, int stock)
        {
            _ = skuCode;

            var productBelongsToTenant = await _context.Products
                .AnyAsync(p => p.Id == productId && p.TenantId == tenantId);

            if (!productBelongsToTenant)
            {
                throw new KeyNotFoundException("Product not found for tenant.");
            }

            var sku = new ProductSku
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Price = price,
                Stock = stock
            };

            await _context.ProductSkus.AddAsync(sku);
            await _context.SaveChangesAsync();

            return sku;
        }

        public async Task<ProductSku?> UpdateProductSkuAsync(Guid tenantId, Guid productSkuId, string skuCode, decimal price, int stock)
        {
            _ = skuCode;

            var sku = await _context.ProductSkus
                .Include(ps => ps.Product)
                .FirstOrDefaultAsync(ps => ps.Id == productSkuId && ps.Product.TenantId == tenantId);

            if (sku == null)
            {
                return null;
            }

            sku.Price = price;
            sku.Stock = stock;

            await _context.SaveChangesAsync();
            return sku;
        }

        public async Task<ProductSku?> DeleteProductSkuAsync(Guid tenantId, Guid productSkuId)
        {
            var sku = await _context.ProductSkus
                .Include(ps => ps.Product)
                .FirstOrDefaultAsync(ps => ps.Id == productSkuId && ps.Product.TenantId == tenantId);

            if (sku == null)
            {
                return null;
            }

            _context.ProductSkus.Remove(sku);
            await _context.SaveChangesAsync();

            return sku;
        }
    }
}
