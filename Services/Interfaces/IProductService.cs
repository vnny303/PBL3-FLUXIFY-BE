using FluxifyAPI.DTOs.Product;
using FluxifyAPI.DTOs.ProductSku;
using FluxifyAPI.Helpers;
using FluxifyAPI.Services.Common;

namespace FluxifyAPI.Services.Interfaces
{
    public interface IProductService
    {
        Task<ServiceResult<IEnumerable<ProductDto>>> GetProductsAsync(Guid tenantId, QueryProduct query);
        Task<ServiceResult<ProductDetailDto>> GetProductDetailAsync(Guid tenantId, Guid id);
        Task<ServiceResult<ProductDto>> GetProductAsync(Guid tenantId, Guid id);
        Task<ServiceResult<ProductDto>> CreateProductAsync(Guid tenantId, Guid platformUserId, CreateProductRequestDto createDto);
        Task<ServiceResult<ProductDto>> UpdateProductAsync(Guid tenantId, Guid platformUserId, Guid productId, UpdateProductRequestDto updateDto);
        Task<ServiceResult<object>> DeleteProductAsync(Guid tenantId, Guid platformUserId, Guid productId);

        Task<ServiceResult<IEnumerable<ProductSkuDto>>> GetSkusAsync(Guid tenantId, Guid productId);
        Task<ServiceResult<ProductSkuDto>> CreateSkuAsync(Guid tenantId, Guid platformUserId, Guid productId, CreateProductSkuRequestDto createDto);
        Task<ServiceResult<ProductSkuDto>> UpdateSkuAsync(Guid tenantId, Guid platformUserId, Guid productId, Guid skuId, UpdateProductSkuRequestDto updateDto);
        Task<ServiceResult<object>> DeleteSkuAsync(Guid tenantId, Guid platformUserId, Guid productId, Guid skuId);
    }
}


