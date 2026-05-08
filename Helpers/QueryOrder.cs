using Microsoft.AspNetCore.Mvc;

namespace FluxifyAPI.Helpers
{
    public class QueryOrder : QueryBase
    {
        [FromQuery(Name = "customerId")]
        public Guid? CustomerId { get; set; } // TODO: đổi thành Name, Phone, Email,...
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
        private double? _totalFrom;
        [FromQuery(Name = "totalFrom")]
        public double? TotalFrom
        {
            get => _totalFrom;
            set => _totalFrom = value <= 0 ? null : value;
        }
        private double? _totalTo;
        [FromQuery(Name = "totalTo")]
        public double? TotalTo
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


