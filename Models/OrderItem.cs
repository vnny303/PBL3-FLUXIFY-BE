using System;
using System.Collections.Generic;

namespace FluxifyAPI.Models;

public partial class OrderItem
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Guid ProductId { get; set; }

    public string? SelectedOptions { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public Order Order { get; set; } = null!;

    public Product Product { get; set; } = null!;
}
