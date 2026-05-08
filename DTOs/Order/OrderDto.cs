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
        public string? PaymentReference { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public string? BankName { get; set; }
        public string? BankCode { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankAccountName { get; set; }
        public string? TransferContent { get; set; }
        public double TotalAmount { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? ShippingMethod { get; set; }
        public double ShippingFee { get; set; }
        public string? Status { get; set; }
        public double Subtotal { get; set; }
        public double TaxAmount { get; set; }
        public string? OrderNote { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
}

