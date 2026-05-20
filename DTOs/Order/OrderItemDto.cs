using System;

namespace FluxifyAPI.DTOs.Order
{
    public class OrderItemDto
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid ProductSkuId { get; set; }

        public string? SelectedOptions { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}

