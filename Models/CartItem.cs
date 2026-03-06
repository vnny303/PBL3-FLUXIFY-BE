using System;
using System.Collections.Generic;

namespace FluxifyAPI.Models;

public partial class CartItem
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public Guid ProductId { get; set; }

    public string? SelectedOptions { get; set; }

    public int Quantity { get; set; }

    public Customer Customer { get; set; } = null!;

    public Product Product { get; set; } = null!;
}
