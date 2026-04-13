using FluxifyAPI.DTOs.Product;
using FluxifyAPI.DTOs.ProductSku;
using FluxifyAPI.Helpers;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Mapper;
using FluxifyAPI.Services.Interfaces;
using FluxifyAPI.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Services.Implementations {
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductSkuRepository _productSkuRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(
            IProductRepository productRepository,
            IProductSkuRepository productSkuRepository,
            ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _productSkuRepository = productSkuRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<ServiceResult<IEnumerable<ProductDto>>> GetProductsAsync(Guid tenantId, QueryProduct query)
        {
            if (query.TenantId.HasValue && query.TenantId.Value != tenantId)
                return ServiceResult<IEnumerable<ProductDto>>.Fail(400, "tenantId trong query không khớp route");

            var productQuery = _productRepository.GetProductsByTenant(tenantId);

            var searchTerm = query.NormalizedSearchTerm;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                if (Guid.TryParse(searchTerm, out var productId))
                {
                    productQuery = productQuery.Where(p =>
                        p.Id == productId ||
                        p.Name.Contains(searchTerm) ||
                        (p.Description != null && p.Description.Contains(searchTerm)));
                }
                else
                {
                    productQuery = productQuery.Where(p =>
                        p.Name.Contains(searchTerm) ||
                        (p.Description != null && p.Description.Contains(searchTerm)));
                }
            }

            if (query.CategoryId.HasValue)
                productQuery = productQuery.Where(p => p.CategoryId == query.CategoryId.Value);

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                var name = query.Name.Trim();
                productQuery = productQuery.Where(p => p.Name.Contains(name));
            }

            if (query.HasAttributes.HasValue)
            {
                productQuery = query.HasAttributes.Value
                    ? productQuery.Where(p => p.Attributes != null && p.Attributes != string.Empty)
                    : productQuery.Where(p => p.Attributes == null || p.Attributes == string.Empty);
            }

            var sortBy = query.SortBy?.Trim();
            var isDescending = query.NormalizedIsDescending;
            var normalizedSortBy = sortBy?.ToLowerInvariant();

            if (normalizedSortBy == "name")
                productQuery = isDescending ? productQuery.OrderByDescending(p => p.Name) : productQuery.OrderBy(p => p.Name);
            else if (normalizedSortBy == "categoryid" || normalizedSortBy == "category_id")
                productQuery = isDescending ? productQuery.OrderByDescending(p => p.CategoryId) : productQuery.OrderBy(p => p.CategoryId);
            else if (normalizedSortBy == "hasattributes" || normalizedSortBy == "has_attributes")
                productQuery = isDescending
                    ? productQuery.OrderByDescending(p => p.Attributes != null && p.Attributes != string.Empty)
                    : productQuery.OrderBy(p => p.Attributes != null && p.Attributes != string.Empty);
            else if (normalizedSortBy == "id")
                productQuery = isDescending ? productQuery.OrderByDescending(p => p.Id) : productQuery.OrderBy(p => p.Id);
            else
                productQuery = productQuery.OrderBy(p => p.Id);

            var pageNumber = query.NormalizedPageNumber;
            var pageSize = query.NormalizedPageSize;
            var skipNumber = (pageNumber - 1) * pageSize;

            var products = await productQuery.Skip(skipNumber).Take(pageSize).ToListAsync();
            return ServiceResult<IEnumerable<ProductDto>>.Ok(products.Select(p => p.ToProductDto()));
        }

        public async Task<ServiceResult<ProductDto>> GetProductAsync(Guid tenantId, Guid id)
        {
            var product = await _productRepository.GetProductAsync(tenantId, id);
            if (product == null)
                return ServiceResult<ProductDto>.Fail(404, "Không tìm thấy sản phẩm");

            return ServiceResult<ProductDto>.Ok(product.ToProductDto());
        }

        public async Task<ServiceResult<ProductDto>> CreateProductAsync(Guid tenantId, CreateProductRequestDto createDto)
        {
            var category = await _categoryRepository.GetCategoryAsync(tenantId, createDto.CategoryId);
            if (category == null)
                return ServiceResult<ProductDto>.Fail(400, "Category không tồn tại trong tenant này");

            var product = createDto.ToProductFromCreateDto(tenantId);
            var createdProduct = await _productRepository.CreateProductAsync(product);
            return ServiceResult<ProductDto>.Ok(createdProduct.ToProductDto());
        }

        public async Task<ServiceResult<ProductDto>> UpdateProductAsync(Guid tenantId, Guid id, UpdateProductRequestDto updateDto)
        {
            var product = await _productRepository.GetProductAsync(tenantId, id);
            if (product == null)
                return ServiceResult<ProductDto>.Fail(404, "Khong tim thay san pham!");

            if (updateDto.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetCategoryAsync(tenantId, updateDto.CategoryId.Value);
                if (category == null)
                    return ServiceResult<ProductDto>.Fail(400, "Category không tồn tại trong tenant này");
            }

            updateDto.ToProductFromUpdateDto(product);
            var updatedProduct = await _productRepository.UpdateProductAsync(product);
            return ServiceResult<ProductDto>.Ok(updatedProduct.ToProductDto());
        }

        public async Task<ServiceResult<object>> DeleteProductAsync(Guid tenantId, Guid id)
        {
            try
            {
                var product = await _productRepository.DeleteProductAsync(tenantId, id);
                if (product == null)
                    return ServiceResult<object>.Fail(404, "Khong tim thay san pham!");

                return ServiceResult<object>.Ok(new { message = "Xoa thanh cong!" });
            }
            catch (DbUpdateException)
            {
                return ServiceResult<object>.Fail(400, "Không thể xóa sản phẩm vì SKU đang được tham chiếu trong giỏ hàng hoặc đơn hàng");
            }
        }

        public async Task<ServiceResult<IEnumerable<ProductSkuDto>>> GetSkusAsync(Guid tenantId, Guid productId)
        {
            var product = await _productRepository.GetProductAsync(tenantId, productId);
            if (product == null)
                return ServiceResult<IEnumerable<ProductSkuDto>>.Fail(404, "Khong tim thay san pham!");

            var skus = await _productSkuRepository.GetProductSkusByProductAsync(tenantId, productId) ?? [];
            return ServiceResult<IEnumerable<ProductSkuDto>>.Ok(skus.Select(s => s.ToProductSkuDto()));
        }

        public async Task<ServiceResult<ProductSkuDto>> CreateSkuAsync(Guid tenantId, Guid productId, CreateProductSkuRequestDto createDto)
        {
            var product = await _productRepository.GetProductAsync(tenantId, productId);
            if (product == null)
                return ServiceResult<ProductSkuDto>.Fail(404, "Khong tim thay san pham!");

            var sku = createDto.ToProductSkuFromCreateDto(productId);
            var createdSku = await _productSkuRepository.CreateProductSkuAsync(sku);
            return ServiceResult<ProductSkuDto>.Ok(createdSku.ToProductSkuDto());
        }

        public async Task<ServiceResult<ProductSkuDto>> UpdateSkuAsync(Guid tenantId, Guid productId, Guid skuId, UpdateProductSkuRequestDto updateDto)
        {
            var sku = await _productSkuRepository.GetProductSkuAsync(tenantId, skuId);
            if (sku == null || sku.ProductId != productId)
                return ServiceResult<ProductSkuDto>.Fail(404, "Khong tim thay SKU!");

            updateDto.ToProductSkuFromUpdateDto(sku);
            var updatedSku = await _productSkuRepository.UpdateProductSkuAsync(sku);
            return ServiceResult<ProductSkuDto>.Ok(updatedSku.ToProductSkuDto());
        }

        public async Task<ServiceResult<object>> DeleteSkuAsync(Guid tenantId, Guid productId, Guid skuId)
        {
            var sku = await _productSkuRepository.GetProductSkuAsync(tenantId, skuId);
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




