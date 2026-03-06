using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxifyAPI.DTOs.Cartegory;
using FluxifyAPI.DTOs.Product;
using FluxifyAPI.Models;

namespace FluxifyAPI.Mapper
{
    public static class CategoryMapper
    {
        public static CategoryDto ToCategoryDto(this Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                TenantId = category.TenantId,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                Products = category.Products?.Select(p => p.ToProductDto()).ToList() ?? new List<ProductDto>()
            };
        }
    }
}