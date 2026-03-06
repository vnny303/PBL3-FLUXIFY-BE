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
    }
}