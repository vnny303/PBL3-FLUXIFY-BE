using System;
using System.Collections.Generic;
using System.Linq;
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
                AddressId = order.AddressId,
                Status = order.Status,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                OrderCode = order.OrderCode,
                OrderNote = order.OrderNote,
                ShippingMethod = order.ShippingMethod,
                Subtotal = order.Subtotal,
                ShippingFee = order.ShippingFee,
                TaxAmount = order.TaxAmount,
                TotalAmount = order.TotalAmount,
                PaidAt = order.PaidAt,
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

        public static Order ToOrderFromUpdateStatusDto(this UpdateOrderStatusRequestDto updateDto, Order existingOrder)
        {
            existingOrder.Status = updateDto.Status.Trim();
            return existingOrder;
        }
    }
}

