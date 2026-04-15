using Microsoft.AspNetCore.Mvc;

namespace FluxifyAPI.Helpers
{
    public class QueryCartItem : QueryBase
    {
        [FromQuery(Name = "cartId")]
        public Guid? CartId { get; set; }

        [FromQuery(Name = "productSkuId")]
        public Guid? ProductSkuId { get; set; }

        [FromQuery(Name = "quantityFrom")]
        public int? QuantityFrom { get; set; }

        [FromQuery(Name = "quantityTo")]
        public int? QuantityTo { get; set; }
    }
}


