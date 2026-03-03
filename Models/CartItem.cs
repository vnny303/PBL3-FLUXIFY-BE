using System;
using System.Collections.Generic;

namespace ShopifyAPI.Models;

public partial class CartItem
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public Guid ProductId { get; set; }

    public string? SelectedOptions { get; set; }

    public int Quantity { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
