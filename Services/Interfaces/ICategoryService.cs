using FluxifyAPI.DTOs.Cartegory;
using FluxifyAPI.Helpers;
using FluxifyAPI.Services;

namespace FluxifyAPI.IServices
{
    public interface ICategoryService
    {
        Task<ServiceResult<IEnumerable<CategoryDto>>> GetCategoriesAsync(Guid tenantId, QueryCategory query);
        Task<ServiceResult<CategoryDto>> CreateCategoryAsync(Guid tenantId, CreateCategoryRequestDto createDto);
        Task<ServiceResult<CategoryDto>> UpdateCategoryAsync(Guid tenantId, Guid categoryId, UpdateCategoryRequestDto updateDto);
        Task<ServiceResult<object>> DeleteCategoryAsync(Guid tenantId, Guid categoryId);
    }
}
