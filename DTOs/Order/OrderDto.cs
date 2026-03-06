using System;
using System.Collections.Generic;

namespace FluxifyAPI.DTOs.Order
{
    public class OrderDto
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        public Guid? CustomerId { get; set; }

        public string? Address { get; set; }

        public string? Status { get; set; }

        public string? PaymentMethod { get; set; }

        public string? PaymentStatus { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime? CreatedAt { get; set; }

        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
}