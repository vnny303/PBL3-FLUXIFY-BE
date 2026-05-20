using Microsoft.AspNetCore.Mvc;

namespace FluxifyAPI.Helpers
{
    public class QueryProduct : QueryBase
    {
        [FromQuery(Name = "categoryId")]
        public Guid? CategoryId { get; set; }
        private string? _name;
        [FromQuery(Name = "name")]
        public string? Name
        {
            get => _name;
            set => _name = value?.Trim().ToLowerInvariant();
        }

        [FromQuery(Name = "hasAttributes")]
        public bool? HasAttributes { get; set; }

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

    }
}


