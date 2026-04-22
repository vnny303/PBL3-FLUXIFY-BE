using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxifyAPI.DTOs.Cart;
using FluxifyAPI.Models;

namespace FluxifyAPI.Mapper
{
    public static class CartMapper
    {
        public static Cart ToCartFromCreateDto(this CreateCartRequestDto createDto)
        {
            return new Cart
            {
                Id = Guid.NewGuid(),
                TenantId = createDto.TenantId,
                CustomerId = createDto.CustomerId
            };
        }

        public static CartDto ToCartDto(this Cart cart)
        {
            return new CartDto
            {
                Id = cart.Id,
                CustomerId = cart.CustomerId,
                TenantId = cart.TenantId,
                CartItems = cart.CartItems?.Select(ci => ci.ToCartItemDto()).ToList() ?? new List<CartItemDto>()
            };
        }
        public static CartItemDto ToCartItemDto(this CartItem cartItem)
        {
            var productName = cartItem.ProductSku?.Product?.Name;
            var skuAttributes = cartItem.ProductSku?.Attributes;
            var skuDisplayName = string.IsNullOrWhiteSpace(productName)
                ? null
                : skuAttributes == null
                    ? productName
                    : $"{productName} - {skuAttributes}";

            return new CartItemDto
            {
                Id = cartItem.Id,
                CartId = cartItem.CartId,
                ProductSkuId = cartItem.ProductSkuId,
                ProductName = productName,
                SkuAttributes = skuAttributes,
                SkuDisplayName = skuDisplayName,
                SkuImageUrl = cartItem.ProductSku?.imgUrl,
                UnitPrice = cartItem.ProductSku?.Price,
                Quantity = cartItem.Quantity
            };
        }

        public static CartItem ToCartItemFromCreateDto(this CreateCartItemRequestDto createDto, Guid cartId)
        {
            return new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cartId,
                ProductSkuId = createDto.ProductSkuId,
                Quantity = createDto.Quantity
            };
        }
    }
}

