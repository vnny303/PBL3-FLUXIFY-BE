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
        public static Cart ToCartFromCreateDto(this CreateCartRequestDto createDto, Guid tenantId, Guid customerId)
        {
            return new Cart
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CustomerId = customerId
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
            return new CartItemDto
            {
                Id = cartItem.Id,
                CartId = cartItem.CartId,
                ProductSkuId = cartItem.ProductSkuId,
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

        public static CartItem ToCartItemFromUpdateDto(this UpdateCartItemRequestDto updateDto, CartItem existingCartItem)
        {
            existingCartItem.Quantity = updateDto.Quantity;
            return existingCartItem;
        }
    }
}