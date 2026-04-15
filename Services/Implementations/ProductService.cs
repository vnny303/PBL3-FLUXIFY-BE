using FluxifyAPI.DTOs.Product;
using FluxifyAPI.DTOs.ProductSku;
using FluxifyAPI.Helpers;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Mapper;
using FluxifyAPI.Services.Interfaces;
using FluxifyAPI.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductSkuRepository _productSkuRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITenantRepository _tenantRepository;

        public ProductService(
            IProductRepository productRepository,
            IProductSkuRepository productSkuRepository,
            ICategoryRepository categoryRepository,
            ITenantRepository tenantRepository)
        {
            _productRepository = productRepository;
            _productSkuRepository = productSkuRepository;
            _categoryRepository = categoryRepository;
            _tenantRepository = tenantRepository;
        }
        public async Task<ServiceResult<IEnumerable<ProductDto>>> GetProductsAsync(Guid tenantId, QueryProduct query)
        {
            var productQuery = _productRepository.GetProductsByTenant(tenantId);

            var searchTerm = query.SearchTerm;
            if (!string.IsNullOrEmpty(searchTerm))
                productQuery = productQuery.Where(p =>
                    p.Name.Contains(searchTerm) ||
                    (p.Description != null && p.Description.Contains(searchTerm)));
            if (!string.IsNullOrWhiteSpace(query.Name))
                productQuery = productQuery.Where(p => p.Name.Contains(query.Name.Trim()));
            if (query.HasAttributes.HasValue)
                productQuery = query.HasAttributes.Value
                    ? productQuery.Where(p => p.Attributes != null && p.Attributes != string.Empty)
                    : productQuery.Where(p => p.Attributes == null || p.Attributes == string.Empty);

            var sortBy = query.SortBy;
            var isDescending = string.Equals(query.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                switch (sortBy)
                {
                    case "name":
                        productQuery = isDescending ? productQuery.OrderByDescending(p => p.Name) : productQuery.OrderBy(p => p.Name);
                        break;
                    case "categoryid":
                    case "category_id":
                        productQuery = isDescending ? productQuery.OrderByDescending(p => p.CategoryId) : productQuery.OrderBy(p => p.CategoryId);
                        break;
                    case "hasattributes":
                    case "has_attributes":
                        productQuery = isDescending
                            ? productQuery.OrderByDescending(p => p.Attributes != null && p.Attributes != string.Empty)
                            : productQuery.OrderBy(p => p.Attributes != null && p.Attributes != string.Empty);
                        break;
                    case "id":
                        productQuery = isDescending ? productQuery.OrderByDescending(p => p.Id) : productQuery.OrderBy(p => p.Id);
                        break;
                    default:
                        productQuery = productQuery.OrderBy(p => p.Id);
                        break;
                }
            }
            var skipNumber = (query.Page - 1) * query.PageSize;

            var products = await productQuery.Skip(skipNumber).Take(query.PageSize).ToListAsync();
            return ServiceResult<IEnumerable<ProductDto>>.Ok(products.Select(p => p.ToProductDto()));
        }

        public async Task<ServiceResult<ProductDto>> GetProductAsync(Guid tenantId, Guid id)
        {
            var product = await _productRepository.GetProductAsync(tenantId, id);
            if (product == null)
                return ServiceResult<ProductDto>.Fail(404, "Không tìm thấy sản phẩm");

            return ServiceResult<ProductDto>.Ok(product.ToProductDto());
        }

        public async Task<ServiceResult<ProductDto>> CreateProductAsync(Guid tenantId, Guid platformUserId, CreateProductRequestDto createDto)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<ProductDto>.Forbidden("Bạn không có quyền đối với sản phẩm này");
            var category = await _categoryRepository.GetCategoryAsync(tenantId, createDto.CategoryId);
            if (category == null)
                return ServiceResult<ProductDto>.Fail(400, "Category không tồn tại trong tenant này");
            var product = createDto.ToProductFromCreateDto(tenantId);
            var createdProduct = await _productRepository.CreateProductAsync(product);
            foreach (var productSkuDto in createDto.Skus)
            {
                var sku = productSkuDto.ToProductSkuFromCreateDto(createdProduct.Id);
                await _productSkuRepository.CreateProductSkuAsync(sku);
            }
            return ServiceResult<ProductDto>.Ok(createdProduct.ToProductDto());
        }

        public async Task<ServiceResult<ProductDto>> UpdateProductAsync(Guid tenantId, Guid platformUserId, Guid productId, UpdateProductRequestDto updateDto)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<ProductDto>.Forbidden("Bạn không có quyền đối với sản phẩm này");
            var product = await _productRepository.GetProductAsync(tenantId, productId);
            if (product == null)
                return ServiceResult<ProductDto>.Fail(404, "không tìm thấy sản phẩm!");
            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<ProductDto>.Forbidden("Bạn không có quyền đối với sản phẩm này");
            if (updateDto.CategoryId.HasValue && !await _categoryRepository.IsCategoryExists(tenantId, updateDto.CategoryId.Value))
                return ServiceResult<ProductDto>.Fail(400, "Category không tồn tại trong tenant này");
            updateDto.ToProductFromUpdateDto(product);
            var updatedProduct = await _productRepository.UpdateProductAsync(product);
            return ServiceResult<ProductDto>.Ok(updatedProduct.ToProductDto());
        }

        public async Task<ServiceResult<object>> DeleteProductAsync(Guid tenantId, Guid platformUserId, Guid productId)
        {
            try
            {
                if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                    return ServiceResult<object>.Forbidden("Bạn không có quyền đối với sản phẩm này");
                var product = await _productRepository.DeleteProductAsync(tenantId, productId);
                if (product == null)
                    return ServiceResult<object>.Fail(404, "không tìm thấy sản phẩm!");
                return ServiceResult<object>.Ok(new { message = "Xoa thanh cong!" });
            }
            catch (DbUpdateException)
            {
                return ServiceResult<object>.Fail(400, "Không thể xóa sản phẩm vì SKU đang được tham chiếu trong giỏ hàng hoặc đơn hàng");
            }
        }

        public async Task<ServiceResult<IEnumerable<ProductSkuDto>>> GetSkusAsync(Guid tenantId, Guid productId)
        {
            if (!await _productRepository.IsProductExists(tenantId, productId))
                return ServiceResult<IEnumerable<ProductSkuDto>>.Fail(404, "Không tìm thấy sản phẩm!");
            var product = await _productRepository.GetProductAsync(tenantId, productId);
            var skus = await _productSkuRepository.GetProductSkusByProductAsync(tenantId, productId) ?? [];
            return ServiceResult<IEnumerable<ProductSkuDto>>.Ok(skus.Select(s => s.ToProductSkuDto()));
        }

        public async Task<ServiceResult<ProductSkuDto>> CreateSkuAsync(Guid tenantId, Guid platformUserId, Guid productId, CreateProductSkuRequestDto createDto)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<ProductSkuDto>.Forbidden("Bạn không có quyền đối với sản phẩm này");
            if (!await _productRepository.IsProductExists(tenantId, productId))
                return ServiceResult<ProductSkuDto>.Fail(404, "Không tìm thấy sản phẩm!");
            var product = await _productRepository.GetProductAsync(tenantId, productId);
            var sku = createDto.ToProductSkuFromCreateDto(productId);
            var createdSku = await _productSkuRepository.CreateProductSkuAsync(sku);
            return ServiceResult<ProductSkuDto>.Ok(createdSku.ToProductSkuDto());
        }

        public async Task<ServiceResult<ProductSkuDto>> UpdateSkuAsync(Guid tenantId, Guid platformUserId, Guid productId, Guid skuId, UpdateProductSkuRequestDto updateDto)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<ProductSkuDto>.Forbidden("Bạn không có quyền đối với sản phẩm này");
            if (!await _productRepository.IsProductExists(tenantId, productId))
                return ServiceResult<ProductSkuDto>.Fail(404, "Không tìm thấy sản phẩm!");
            var sku = await _productSkuRepository.GetProductSkusAsync(tenantId, skuId);
            if (sku == null || sku.ProductId != productId)
                return ServiceResult<ProductSkuDto>.Fail(404, "Không tìm thấy SKU!");
            updateDto.ToProductSkuFromUpdateDto(sku);
            var updatedSku = await _productSkuRepository.UpdateProductSkuAsync(sku);
            return ServiceResult<ProductSkuDto>.Ok(updatedSku.ToProductSkuDto());
        }

        public async Task<ServiceResult<object>> DeleteSkuAsync(Guid tenantId, Guid platformUserId, Guid productId, Guid skuId)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<object>.Forbidden("Bạn không có quyền đối với sản phẩm này");
            if (!await _productRepository.IsProductExists(tenantId, productId))
                return ServiceResult<object>.Fail(404, "Không tìm thấy sản phẩm!");
            var sku = await _productSkuRepository.GetProductSkusAsync(tenantId, skuId);
            if (sku == null || sku.ProductId != productId)
                return ServiceResult<object>.Fail(404, "Không tìm thấy SKU!");

            try
            {
                await _productSkuRepository.DeleteProductSkuAsync(tenantId, skuId);
                return ServiceResult<object>.Ok(new { message = "Xóa SKU thành công!" });
            }
            catch (DbUpdateException)
            {
                return ServiceResult<object>.Fail(400, "Không thể xóa SKU vì đang được tham chiếu trong giỏ hàng hoặc đơn hàng");
            }
        }
    }
}




