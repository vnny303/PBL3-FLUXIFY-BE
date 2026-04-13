using FluxifyAPI.Models;

namespace FluxifyAPI.Repository.Interfaces
{
    public interface IProductSkuRepository
    {
        public Task<ProductSku?> GetProductSkuAsync(Guid tenantId, Guid productSkuId);
        public Task<IEnumerable<ProductSku>?> GetProductSkusByProductAsync(Guid tenantId, Guid productId);
        public Task<ProductSku> CreateProductSkuAsync(ProductSku productSku);
        public Task<ProductSku> UpdateProductSkuAsync(ProductSku productSku);
        public Task<ProductSku?> DeleteProductSkuAsync(Guid tenantId, Guid productSkuId);

    }
}

