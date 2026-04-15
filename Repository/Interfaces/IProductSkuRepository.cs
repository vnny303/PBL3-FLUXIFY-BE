using FluxifyAPI.Models;

namespace FluxifyAPI.Repository.Interfaces
{
    public interface IProductSkuRepository
    {
        Task<ProductSku?> GetProductSkusAsync(Guid tenantId, Guid productSkuId);
        Task<IEnumerable<ProductSku>?> GetProductSkusByProductAsync(Guid tenantId, Guid productId);
        Task<ProductSku> CreateProductSkuAsync(ProductSku productSku);
        Task<ProductSku> UpdateProductSkuAsync(ProductSku productSku);
        Task<ProductSku?> DeleteProductSkuAsync(Guid tenantId, Guid productSkuId);
        Task<bool> ProductSkuExists(Guid tenantId, Guid productSkuId);


    }
}