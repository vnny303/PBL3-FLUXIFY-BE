using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxifyAPI.Models;

namespace FluxifyAPI.Interfaces
{
    public interface IProductRepository
    {
        public Task<Product?> GetProductAsync(Guid tenantId, Guid productId);
        public Task<Product?> GetProductByCategoryAsync(Guid tenantId, Guid categoryId);
        public IQueryable<Product> GetProductsByTenant(Guid tenantId);
        public Task<List<Product>?> GetProductsByTenantAsync(Guid tenantId);
        public Task<Product> CreateProductAsync(Product product);
        public Task<Product> UpdateProductAsync(Product product);
        public Task<Product?> DeleteProductAsync(Guid tenantId, Guid productId);
    }
}