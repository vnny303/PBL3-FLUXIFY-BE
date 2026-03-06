using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FluxifyAPI.Models
{
    [Table("orders")]
    public class Order
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("tenant_id")]
        public Guid TenantId { get; set; }
        [Column("customer_id")]
        public Guid? CustomerId { get; set; }
        [Column("address")]
        public string? Address { get; set; }
        [Column("status")]
        public string? Status { get; set; }
        [Column("payment_method")]
        public string? PaymentMethod { get; set; }
        [Column("payment_status")]
        public string? PaymentStatus { get; set; }
        [Column("total_amount")]
        public decimal TotalAmount { get; set; }
        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
        [JsonIgnore]
        public Customer Customer { get; set; } = null!;
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        [JsonIgnore]
        public Tenant Tenant { get; set; } = null!;
    }
}
