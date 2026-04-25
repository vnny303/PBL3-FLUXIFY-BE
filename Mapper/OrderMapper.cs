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
                PaymentReference = order.PaymentReference,
                TransferContent = order.TransferContent,
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

        public static Order ToOrderFromCreateDto(this CreateOrderRequestDto createDto, Guid tenantId)
        {
            var orderId = Guid.NewGuid();

            var order = new Order
            {
                Id = orderId,
                TenantId = tenantId,
                CustomerId = createDto.CustomerId,
                AddressId = createDto.AddressId,
                Status = "Pending",
                PaymentMethod = string.IsNullOrWhiteSpace(createDto.PaymentMethod) ? "COD" : createDto.PaymentMethod.Trim(),
                PaymentStatus = string.IsNullOrWhiteSpace(createDto.PaymentStatus) ? "Pending" : createDto.PaymentStatus.Trim(),
                ShippingMethod = "standard",
                Subtotal = createDto.OrderItems.Sum(i => i.UnitPrice * i.Quantity),
                ShippingFee = 0,
                TaxAmount = 0,
                CreatedAt = DateTime.UtcNow,
                OrderItems = createDto.OrderItems.Select(i => i.ToOrderItemFromCreateDto(orderId)).ToList()
            };

            order.TotalAmount = order.Subtotal;
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

