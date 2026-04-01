using FluxifyAPI.Models;

namespace FluxifyAPI.Interfaces
{
    public interface IProductSkuRepository
    {
        public Task<ProductSku?> GetProductSkuAsync(Guid tenantId, Guid productSkuId);
        public Task<List<ProductSku>?> GetProductSkusByProductAsync(Guid tenantId, Guid productId);
        public Task<ProductSku> CreateProductSkuAsync(Guid tenantId, Guid productId, string skuCode, decimal price, int stock);
        public Task<ProductSku?> UpdateProductSkuAsync(Guid tenantId, Guid productSkuId, string skuCode, decimal price, int stock);
        public Task<ProductSku?> DeleteProductSkuAsync(Guid tenantId, Guid productSkuId);

    }
}