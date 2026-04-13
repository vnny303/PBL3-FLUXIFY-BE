using FluxifyAPI.Data;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Repository.Implementations {
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

        public async Task<IEnumerable<ProductSku>?> GetProductSkusByProductAsync(Guid tenantId, Guid productId)
        {
            return await _context.ProductSkus
                .Include(ps => ps.Product)
                .Where(ps => ps.ProductId == productId && ps.Product.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<ProductSku> CreateProductSkuAsync(ProductSku productSku)
        {
            await _context.ProductSkus.AddAsync(productSku);
            await _context.SaveChangesAsync();
            return productSku;
        }

        public async Task<ProductSku> UpdateProductSkuAsync(ProductSku productSku)
        {
            if (_context.Entry(productSku).State == EntityState.Detached)
                _context.ProductSkus.Attach(productSku);

            await _context.SaveChangesAsync();
            return productSku;
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


