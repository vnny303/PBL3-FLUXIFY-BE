using Microsoft.AspNetCore.Mvc;

namespace FluxifyAPI.Helpers
{
    public class QueryOrderItem : QueryBase
    {
        [FromQuery(Name = "orderId")]
        public Guid? OrderId { get; set; }

        [FromQuery(Name = "productSkuId")]
        public Guid? ProductSkuId { get; set; }

        [FromQuery(Name = "quantityFrom")]
        public int? QuantityFrom { get; set; }

        [FromQuery(Name = "quantityTo")]
        public int? QuantityTo { get; set; }

        [FromQuery(Name = "unitPriceFrom")]
        public decimal? UnitPriceFrom { get; set; }

        [FromQuery(Name = "unitPriceTo")]
        public decimal? UnitPriceTo { get; set; }
    }
}
