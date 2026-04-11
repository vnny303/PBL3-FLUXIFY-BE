using Microsoft.AspNetCore.Mvc;

namespace FluxifyAPI.Helpers
{
    public class QueryProductSku : QueryBase
    {
        [FromQuery(Name = "productId")]
        public Guid? ProductId { get; set; }

        [FromQuery(Name = "priceFrom")]
        public decimal? PriceFrom { get; set; }

        [FromQuery(Name = "priceTo")]
        public decimal? PriceTo { get; set; }

        [FromQuery(Name = "stockFrom")]
        public int? StockFrom { get; set; }

        [FromQuery(Name = "stockTo")]
        public int? StockTo { get; set; }
    }
}
