using FluxifyAPI.DTOs.Cartegory;
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
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductSkuRepository _productSkuRepository;
        private readonly ITenantRepository _tenantRepository;

        public CategoryService(ICategoryRepository categoryRepository,
                                IProductRepository productRepository,
                                IProductSkuRepository productSkuRepository,
                                ITenantRepository tenantRepository)
        {
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
            if (!await _categoryRepository.CategoryExists(tenantId, categoryId))
                return ServiceResult<object>.Fail(404, "Không tìm thấy danh mục!");
            foreach (var product in (await _categoryRepository.GetCategoryAsync(tenantId, categoryId)).Products ?? Enumerable.Empty<Product>())
            {
                foreach (var productSku in await _productSkuRepository.GetProductSkusByProductAsync(tenantId, product.Id) ?? Enumerable.Empty<ProductSku>())
                    await _productSkuRepository.DeleteProductSkuAsync(tenantId, productSku.Id);
                await _productRepository.DeleteProductAsync(tenantId, product.Id);
            }
            await _categoryRepository.DeleteCategoryAsync(tenantId, categoryId);
            return ServiceResult<object>.Ok(new { message = "Xóa thành công!" });
        }
    }
}