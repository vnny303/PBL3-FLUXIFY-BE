using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxifyAPI.DTOs.ProductSku;
using FluxifyAPI.Models;

namespace FluxifyAPI.Mapper
{
    public static class ProductSkuMapper
    {
        public static ProductSkuDto ToProductSkuDto(this ProductSku productSku)
        {
            return new ProductSkuDto
            {
                Id = productSku.Id,
                ProductId = productSku.ProductId,
                Price = productSku.Price,
                Stock = productSku.Stock,
                Attributes = productSku.Attributes
            };
        }
    }
}