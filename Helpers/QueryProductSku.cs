using Microsoft.AspNetCore.Mvc;

namespace FluxifyAPI.Helpers
{
    public class QueryProductSku : QueryBase
    {
        [FromQuery(Name = "productId")]
        public Guid? ProductId { get; set; }
        private decimal? _priceFrom;
        [FromQuery(Name = "priceFrom")]
        public decimal? PriceFrom
        {
            get => _priceFrom;
            set => _priceFrom = value <= 0 ? null : value;
        }
        private decimal? _priceTo;
        [FromQuery(Name = "priceTo")]
        public decimal? PriceTo
        {
            get => _priceTo;
            set => _priceTo = value <= 0 ? null : value;
        }
        private int? _stockFrom;
        [FromQuery(Name = "stockFrom")]
        public int? StockFrom
        {
            get => _stockFrom;
            set => _stockFrom = value < 0 ? null : value;
        }
        private int? _stockTo;
        [FromQuery(Name = "stockTo")]
        public int? StockTo
        {
            get => _stockTo;
            set => _stockTo = value < 0 ? null : value;
        }
    }
}


