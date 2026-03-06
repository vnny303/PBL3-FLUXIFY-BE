using System;
using System.Collections.Generic;

namespace FluxifyAPI.Models;

public partial class OrderItem
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Guid ProductSkuId { get; set; }

    public string? SelectedOptions { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public Order Order { get; set; } = null!;

    public ProductSku ProductSku { get; set; } = null!;
}
