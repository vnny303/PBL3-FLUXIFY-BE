using Microsoft.AspNetCore.Mvc;

namespace FluxifyAPI.Helpers
{
    public class QueryOrder : QueryBase
    {
        [FromQuery(Name = "customerId")]
        public Guid? CustomerId { get; set; }
        private string? _status;
        [FromQuery(Name = "status")]
        public string? Status
        {
            get => _status;
            set => _status = value?.Trim().ToLowerInvariant();
        }
        private string? _paymentMethod;
        [FromQuery(Name = "paymentMethod")]
        public string? PaymentMethod
        {
            get => _paymentMethod;
            set => _paymentMethod = value?.Trim().ToLowerInvariant();
        }
        private string? _paymentStatus;
        [FromQuery(Name = "paymentStatus")]
        public string? PaymentStatus
        {
            get => _paymentStatus;
            set => _paymentStatus = value?.Trim().ToLowerInvariant();
        }
        private decimal? _totalFrom;
        [FromQuery(Name = "totalFrom")]
        public decimal? TotalFrom
        {
            get => _totalFrom;
            set => _totalFrom = value <= 0 ? null : value;
        }
        private decimal? _totalTo;
        [FromQuery(Name = "totalTo")]
        public decimal? TotalTo
        {
            get => _totalTo;
            set => _totalTo = value <= 0 ? null : value;
        }
        [FromQuery(Name = "createdFrom")]
        public DateTime? CreatedFrom { get; set; }

        [FromQuery(Name = "createdTo")]
        public DateTime? CreatedTo { get; set; }
    }
}


