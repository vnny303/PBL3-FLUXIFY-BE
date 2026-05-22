using FluxifyAPI.Data;
using FluxifyAPI.DTOs.Category;
using FluxifyAPI.Helpers;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Mapper;
using FluxifyAPI.Services.Interfaces;
using FluxifyAPI.Services.Common;
using Microsoft.EntityFrameworkCore;
using FluxifyAPI.Models;

namespace FluxifyAPI.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductSkuRepository _productSkuRepository;
        private readonly ITenantRepository _tenantRepository;

        public CategoryService(AppDbContext context,
                                ICategoryRepository categoryRepository,
                                IProductRepository productRepository,
                                IProductSkuRepository productSkuRepository,
                                ITenantRepository tenantRepository)
        {
            _context = context;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _productSkuRepository = productSkuRepository;
            _tenantRepository = tenantRepository;
        }

        public async Task<ServiceResult<IEnumerable<CategoryDto>>> GetCategoriesAsync(Guid tenantId, QueryCategory query)
        {
            var categoryQuery = _categoryRepository.GetCategoriesByTenantQuery(tenantId);

            if (!string.IsNullOrEmpty(query.SearchTerm))
                categoryQuery = categoryQuery.Where(c => c.Name.Contains(query.SearchTerm) ||
                    (c.Description != null && c.Description.Contains(query.SearchTerm)));
            if (!string.IsNullOrWhiteSpace(query.Name))
                categoryQuery = categoryQuery.Where(c => c.Name.Contains(query.Name));
            if (!string.IsNullOrWhiteSpace(query.Description))
                categoryQuery = categoryQuery.Where(c => c.Description != null && c.Description.Contains(query.Description));
            if (query.IsActive.HasValue)
                categoryQuery = categoryQuery.Where(c => c.IsActive == query.IsActive.Value);

            var isDescending = string.Equals(query.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
            switch (query.SortBy?.ToLowerInvariant())
            {
                case "name":
                    categoryQuery = isDescending ? categoryQuery.OrderByDescending(c => c.Name) : categoryQuery.OrderBy(c => c.Name);
                    break;
                case "description":
                    categoryQuery = isDescending ? categoryQuery.OrderByDescending(c => c.Description) : categoryQuery.OrderBy(c => c.Description);
                    break;
                case "isactive":
                case "is_active":
                    categoryQuery = isDescending ? categoryQuery.OrderByDescending(c => c.IsActive) : categoryQuery.OrderBy(c => c.IsActive);
                    break;
                case "id":
                    categoryQuery = isDescending ? categoryQuery.OrderByDescending(c => c.Id) : categoryQuery.OrderBy(c => c.Id);
                    break;
                default:
                    categoryQuery = categoryQuery.OrderBy(c => c.Id);
                    break;
            }
            var skipNumber = (query.Page - 1) * query.PageSize;
            var categories = await categoryQuery.Skip(skipNumber).Take(query.PageSize).ToListAsync();

            return ServiceResult<IEnumerable<CategoryDto>>.Ok(categories.Select(c => c.ToCategoryDto()));
        }

        public async Task<ServiceResult<CategoryDto>> GetCategoryByIdAsync(Guid tenantId, Guid categoryId)
        {
            var category = await _categoryRepository.GetCategoryAsync(tenantId, categoryId);
            if (category == null)
                return ServiceResult<CategoryDto>.Fail(404, "Không tìm thấy danh mục!");
            return ServiceResult<CategoryDto>.Ok(category.ToCategoryDto());
        }

        public async Task<ServiceResult<CategoryDto>> CreateCategoryAsync(Guid tenantId, Guid platformUserId, CreateCategoryRequestDto createDto)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<CategoryDto>.Forbidden("Bạn không có quyền đối với danh mục của tenant này!");
            if (await _categoryRepository.CategoryNameExists(tenantId, createDto.Name))
                return ServiceResult<CategoryDto>.Fail(400, "Tên danh mục đã tồn tại!");
            var category = createDto.ToCategoryFromCreateDto(tenantId);
            var createdCategory = await _categoryRepository.CreateCategoryAsync(category);
            return ServiceResult<CategoryDto>.Created(createdCategory.ToCategoryDto());
        }

        public async Task<ServiceResult<CategoryDto>> UpdateCategoryAsync(Guid tenantId, Guid platformUserId, Guid categoryId, UpdateCategoryRequestDto updateDto)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<CategoryDto>.Forbidden("Bạn không có quyền đối với danh mục của tenant này!");
            var category = await _categoryRepository.GetCategoryAsync(tenantId, categoryId);
            if (!await _categoryRepository.CategoryExists(tenantId, categoryId) || category == null)
                return ServiceResult<CategoryDto>.Fail(404, "Không tìm thấy danh mục!");
            if (updateDto.Name != null && await _categoryRepository.CategoryNameExists(tenantId, updateDto.Name))
                return ServiceResult<CategoryDto>.Fail(400, "Tên danh mục đã tồn tại!");
            updateDto.ToCategoryFromUpdateDto(category);
            var updatedCategory = await _categoryRepository.UpdateCategoryAsync(category);
            return ServiceResult<CategoryDto>.Ok(updatedCategory.ToCategoryDto());
        }

        public async Task<ServiceResult<object>> DeleteCategoryAsync(Guid tenantId, Guid platformUserId, Guid categoryId)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<object>.Forbidden("Bạn không có quyền đối với danh mục của tenant này!");

            var category = await _categoryRepository.GetCategoryAsync(tenantId, categoryId);
            if (category == null)
                return ServiceResult<object>.Fail(404, "Không tìm thấy danh mục!");

            var productIds = category.Products.Select(p => p.Id).ToList();

            if (productIds.Count > 0)
            {
                var skuIds = await _context.ProductSkus
                    .AsNoTracking()
                    .Where(ps => productIds.Contains(ps.ProductId))
                    .Select(ps => ps.Id)
                    .ToListAsync();

                // Không xóa category nếu còn order items tham chiếu SKU thuộc category này
                // (giống behavior của ProductService.DeleteProductAsync)
                if (skuIds.Count > 0)
                {
                    var hasOrders = await _context.OrderItems
                        .AsNoTracking()
                        .AnyAsync(oi => skuIds.Contains(oi.ProductSkuId) && oi.Order.TenantId == tenantId);
                    if (hasOrders)
                        return ServiceResult<object>.Fail(400,
                            "Không thể xóa danh mục vì có sản phẩm đang được tham chiếu trong đơn hàng.");
                }
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            if (productIds.Count > 0)
            {
                var skuIds = await _context.ProductSkus
                    .Where(ps => productIds.Contains(ps.ProductId))
                    .Select(ps => ps.Id)
                    .ToListAsync();

                if (skuIds.Count > 0)
                {
                    // Xóa cart items trỏ vào các SKU này (Restrict FK nên phải xóa trước)
                    var cartItems = await _context.CartItems
                        .Where(ci => skuIds.Contains(ci.ProductSkuId))
                        .ToListAsync();
                    _context.CartItems.RemoveRange(cartItems);

                    // Xóa reviews trỏ vào các SKU này
                    var reviews = await _context.Reviews
                        .Where(r => skuIds.Contains(r.ProductSkuId) && r.TenantId == tenantId)
                        .ToListAsync();
                    _context.Reviews.RemoveRange(reviews);

                    // Bulk delete SKUs
                    var skus = await _context.ProductSkus
                        .Where(ps => productIds.Contains(ps.ProductId))
                        .ToListAsync();
                    _context.ProductSkus.RemoveRange(skus);
                }

                // Bulk delete products
                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();
                _context.Products.RemoveRange(products);

                await _context.SaveChangesAsync();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return ServiceResult<object>.Ok(new { message = "Xóa thành công!" });
        }
    }
}