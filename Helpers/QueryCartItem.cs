using Microsoft.AspNetCore.Mvc;

namespace FluxifyAPI.Helpers
{
    public class QueryCartItem : QueryBase
    {
        [FromQuery(Name = "cartId")]
        public Guid? CartId { get; set; }

        [FromQuery(Name = "productSkuId")]
        public Guid? ProductSkuId { get; set; }
        private int? _quantityFrom;

        [FromQuery(Name = "quantityFrom")]
        public int? QuantityFrom
        {
            get => _quantityFrom;
            set
            {
                _quantityFrom = value <= 0 ? null : value;
            }
        }
        private int? _quantityTo;

        [FromQuery(Name = "quantityTo")]
        public int? QuantityTo
        {
            get => _quantityTo;
            set
            {
                _quantityTo = value <= 0 ? null : value;
            }
        }
    }
}


