using System;
using System.Collections.Generic;

namespace FluxifyAPI.Models;

public partial class CartItem
{
    public Guid Id { get; set; }

    public Guid CartId { get; set; }

    public Guid ProductSkuId { get; set; }

    public int Quantity { get; set; }

    public Cart Cart { get; set; } = null!;

    public ProductSku ProductSku { get; set; } = null!;
}
