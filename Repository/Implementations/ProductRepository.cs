using FluxifyAPI.Data;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Repository.Implementations {
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetProductAsync(Guid tenantId, Guid productId)
        {
            return await _context.Products
                .Include(p => p.ProductSkus)
                .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Id == productId);
        }

        public async Task<Product?> GetProductByCategoryAsync(Guid tenantId, Guid categoryId)
        {
            return await _context.Products
                .Include(p => p.ProductSkus)
                .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.CategoryId == categoryId);
        }

        public IQueryable<Product> GetProductsByTenant(Guid tenantId)
        {
            return _context.Products
                .Include(p => p.ProductSkus)
                .Where(p => p.TenantId == tenantId)
                .AsNoTracking();
        }

        public async Task<List<Product>?> GetProductsByTenantAsync(Guid tenantId)
        {
            return await GetProductsByTenant(tenantId).ToListAsync();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            if (_context.Entry(product).State == EntityState.Detached)
                _context.Products.Attach(product);

            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> DeleteProductAsync(Guid tenantId, Guid productId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Id == productId);

            if (product == null)
            {
                return null;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return product;
        }
    }
}


