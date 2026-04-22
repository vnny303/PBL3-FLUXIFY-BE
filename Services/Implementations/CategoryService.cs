using FluxifyAPI.DTOs.Cartegory;
using FluxifyAPI.Helpers;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Mapper;
using FluxifyAPI.Services.Interfaces;
using FluxifyAPI.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITenantRepository _tenantRepository;

        public CategoryService(ICategoryRepository categoryRepository, ITenantRepository tenantRepository)
        {
            _categoryRepository = categoryRepository;
            _tenantRepository = tenantRepository;
        }

        public async Task<ServiceResult<IEnumerable<CategoryDto>>> GetCategoriesAsync(Guid tenantId, QueryCategory query)
        {
            var categoryQuery = _categoryRepository.GetCategoriesByTenant(tenantId);

            var searchTerm = query.SearchTerm;
            if (!string.IsNullOrEmpty(searchTerm))
                categoryQuery = categoryQuery.Where(c =>
                    c.Name.Contains(searchTerm) ||
                    (c.Description != null && c.Description.Contains(searchTerm)));
            if (!string.IsNullOrWhiteSpace(query.Name))
                categoryQuery = categoryQuery.Where(c => c.Name.Contains(query.Name.Trim()));
            if (!string.IsNullOrWhiteSpace(query.Description))
                categoryQuery = categoryQuery.Where(c => c.Description != null && c.Description.Contains(query.Description.Trim()));
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
            if (await _categoryRepository.IsCategoryNameExists(tenantId, createDto.Name))
                return ServiceResult<CategoryDto>.Fail(400, "Tên danh mục đã tồn tại!");
            var category = createDto.ToCategoryFromCreateDto(tenantId);
            var createdCategory = await _categoryRepository.CreateCategoryAsync(category);
            return ServiceResult<CategoryDto>.Created(createdCategory.ToCategoryDto());
        }

        public async Task<ServiceResult<CategoryDto>> UpdateCategoryAsync(Guid tenantId, Guid platformUserId, Guid categoryId, UpdateCategoryRequestDto updateDto)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<CategoryDto>.Forbidden("Bạn không có quyền đối với danh mục của tenant này!");
            if (!await _categoryRepository.IsCategoryExists(tenantId, categoryId))
                return ServiceResult<CategoryDto>.Fail(404, "Không tìm thấy danh mục!");
            var category = await _categoryRepository.GetCategoryAsync(tenantId, categoryId);
            if (updateDto.Name != null && await _categoryRepository.IsCategoryNameExists(tenantId, updateDto.Name))
                return ServiceResult<CategoryDto>.Fail(400, "Tên danh mục đã tồn tại!");
            updateDto.ToCategoryFromUpdateDto(category);
            var updatedCategory = await _categoryRepository.UpdateCategoryAsync(category);
            return ServiceResult<CategoryDto>.Ok(updatedCategory.ToCategoryDto());
        }

        public async Task<ServiceResult<object>> DeleteCategoryAsync(Guid tenantId, Guid platformUserId, Guid categoryId)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<object>.Forbidden("Bạn không có quyền đối với danh mục của tenant này!");
            var deletedCategory = await _categoryRepository.DeleteCategoryAsync(tenantId, categoryId);
            if (deletedCategory == null)
                return ServiceResult<object>.Fail(404, "Không tìm thấy danh mục!");
            return ServiceResult<object>.Ok(new { message = "Xóa thành công!" });
        }
    }
}




