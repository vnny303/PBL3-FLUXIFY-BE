using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxifyAPI.Models;

namespace FluxifyAPI.Repository.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetProductAsync(Guid tenantId, Guid productId);
        public IQueryable<Product> GetProductByCategory(Guid tenantId, Guid categoryId);
        public IQueryable<Product> GetProductsByTenant(Guid tenantId);
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task<Product?> DeleteProductAsync(Guid tenantId, Guid productId);
        Task<bool> IsProductExists(Guid tenantId, Guid productId);
    }
}

