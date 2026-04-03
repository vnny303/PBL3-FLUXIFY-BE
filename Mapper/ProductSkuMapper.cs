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

        public static ProductSku ToProductSkuFromCreateDto(this CreateProductSkuRequestDto createDto, Guid productId)
        {
            return new ProductSku
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Price = createDto.Price,
                Stock = createDto.Stock,
                Attributes = createDto.Attributes
            };
        }

        public static ProductSku ToProductSkuFromUpdateDto(this UpdateProductSkuRequestDto updateDto, ProductSku existingProductSku)
        {
            if (updateDto.Price.HasValue)
            {
                existingProductSku.Price = updateDto.Price.Value;
            }

            if (updateDto.Stock.HasValue)
            {
                existingProductSku.Stock = updateDto.Stock.Value;
            }

            if (updateDto.Attributes != null)
            {
                existingProductSku.Attributes = updateDto.Attributes;
            }

            return existingProductSku;
        }
    }
}