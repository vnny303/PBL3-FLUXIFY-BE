using Microsoft.AspNetCore.Mvc;

namespace FluxifyAPI.Helpers
{
    public class QueryOrderItem : QueryBase
    {
        [FromQuery(Name = "orderId")]
        public Guid? OrderId { get; set; }

        [FromQuery(Name = "productSkuId")]
        public Guid? ProductSkuId { get; set; }
        private int? _quantityFrom;
        [FromQuery(Name = "quantityFrom")]
        public int? QuantityFrom
        {
            get => _quantityFrom;
            set => _quantityFrom = value <= 0 ? null : value;
        }
        private int? _quantityTo;
        [FromQuery(Name = "quantityTo")]
        public int? QuantityTo
        {
            get => _quantityTo;
            set => _quantityTo = value <= 0 ? null : value;
        }
        private double? _unitPriceFrom;
        [FromQuery(Name = "unitPriceFrom")]
        public double? UnitPriceFrom
        {
            get => _unitPriceFrom;
            set => _unitPriceFrom = value <= 0 ? null : value;
        }
        private double? _unitPriceTo;
        [FromQuery(Name = "unitPriceTo")]
        public double? UnitPriceTo
        {
            get => _unitPriceTo;
            set => _unitPriceTo = value <= 0 ? null : value;
        }
    }
}


