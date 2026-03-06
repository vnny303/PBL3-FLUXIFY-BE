using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FluxifyAPI.DTOs.Order;
using FluxifyAPI.Models;

namespace FluxifyAPI.Mapper
{
    public static class OrderMapper
    {
        public static OrderDto ToOrderDto(this Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                TenantId = order.TenantId,
                CustomerId = order.CustomerId,
                Address = order.Address,
                Status = order.Status,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                OrderItems = order.OrderItems.Select(oi => oi.ToOrderItemDto()).ToList()
            };
        }
        public static OrderItemDto ToOrderItemDto(this OrderItem orderItem)
        {
            return new OrderItemDto
            {
                Id = orderItem.Id,
                OrderId = orderItem.OrderId,
                ProductSkuId = orderItem.ProductSkuId,
                SelectedOptions = orderItem.SelectedOptions,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice
            };
        }
    }
}