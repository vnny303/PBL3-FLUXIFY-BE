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

        public static Order ToOrderFromCreateDto(this CreateOrderRequestDto createDto, Guid tenantId)
        {
            var orderId = Guid.NewGuid();

            var order = new Order
            {
                Id = orderId,
                TenantId = tenantId,
                CustomerId = createDto.CustomerId,
                Address = createDto.Address.Trim(),
                Status = "Pending",
                PaymentMethod = string.IsNullOrWhiteSpace(createDto.PaymentMethod) ? "COD" : createDto.PaymentMethod.Trim(),
                PaymentStatus = string.IsNullOrWhiteSpace(createDto.PaymentStatus) ? "Pending" : createDto.PaymentStatus.Trim(),
                CreatedAt = DateTime.UtcNow,
                OrderItems = createDto.OrderItems.Select(i => i.ToOrderItemFromCreateDto(orderId)).ToList()
            };

            order.TotalAmount = order.OrderItems.Sum(i => i.UnitPrice * i.Quantity);
            return order;
        }

        public static OrderItem ToOrderItemFromCreateDto(this CreateOrderItemRequestDto createDto, Guid orderId)
        {
            return new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductSkuId = createDto.ProductSkuId,
                Quantity = createDto.Quantity,
                UnitPrice = createDto.UnitPrice,
                SelectedOptions = null
            };
        }

        public static Order ToOrderFromUpdateStatusDto(this UpdateOrderStatusRequestDto updateDto, Order existingOrder)
        {
            existingOrder.Status = updateDto.Status.Trim();
            return existingOrder;
        }
    }
}