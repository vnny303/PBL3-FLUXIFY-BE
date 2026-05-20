using System;
using System.Collections.Generic;

namespace FluxifyAPI.DTOs.Order
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? AddressId { get; set; }
        public string? OrderCode { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? ShippingMethod { get; set; }
        public decimal ShippingFee { get; set; }
        public string? Status { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public string? OrderNote { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
}

