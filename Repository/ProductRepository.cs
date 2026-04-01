using FluxifyAPI.Data;
using FluxifyAPI.Interfaces;
using FluxifyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Repository
{
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

        public async Task<List<Product>?> GetProductsByTenantAsync(Guid tenantId)
        {
            return await _context.Products
                .Include(p => p.ProductSkus)
                .Where(p => p.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<Product> CreateProductAsync(Guid tenantId, string name, string description)
        {
            var defaultCategoryId = await _context.Categories
                .Where(c => c.TenantId == tenantId)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            if (defaultCategoryId == Guid.Empty)
            {
                throw new InvalidOperationException("Tenant does not have a category to attach product.");
            }

            var product = new Product
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CategoryId = defaultCategoryId,
                Name = name,
                Description = description
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<Product?> UpdateProductAsync(Guid tenantId, Guid productId, string name, string description)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Id == productId);

            if (product == null)
            {
                return null;
            }

            product.Name = name;
            product.Description = description;

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
