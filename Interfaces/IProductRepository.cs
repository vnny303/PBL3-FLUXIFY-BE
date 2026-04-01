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
        public Task<List<Product>?> GetProductsByTenantAsync(Guid tenantId);
        public Task<Product> CreateProductAsync(Guid tenantId, string name, string description);
        public Task<Product?> UpdateProductAsync(Guid tenantId, Guid productId, string name, string description);
        public Task<Product?> DeleteProductAsync(Guid tenantId, Guid productId);
    }
}