using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxifyAPI.DTOs.Product;
using FluxifyAPI.Models;

namespace FluxifyAPI.Mapper
{
    public static class ProductMapper
    {
        public static ProductDto ToProductDto(this Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                TenantId = product.TenantId,
                CategoryId = product.CategoryId,
                Name = product.Name,
                Description = product.Description,
                Attributes = product.Attributes,
                ProductSkus = product.ProductSkus.Select(ps => ps.ToProductSkuDto()).ToList()
            };
        }
    }
}