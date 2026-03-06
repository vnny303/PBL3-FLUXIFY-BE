using System;
using System.Collections.Generic;
using FluxifyAPI.DTOs.Product;

namespace FluxifyAPI.DTOs.Cartegory
{
    public class CategoryDto
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public List<ProductDto>? Products { get; set; } = new List<ProductDto>();
    }
}