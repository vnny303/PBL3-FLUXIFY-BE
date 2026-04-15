using Microsoft.AspNetCore.Mvc;

namespace FluxifyAPI.Helpers
{
    public class QueryOrder : QueryBase
    {
        [FromQuery(Name = "customerId")]
        public Guid? CustomerId { get; set; } // TODO: đổi thành Name, Phone, Email,...

        [FromQuery(Name = "status")]
        public string? Status { get; set; }

        [FromQuery(Name = "paymentMethod")]
        public string? PaymentMethod { get; set; }

        [FromQuery(Name = "paymentStatus")]
        public string? PaymentStatus { get; set; }

        [FromQuery(Name = "totalFrom")]
        public decimal? TotalFrom { get; set; }

        [FromQuery(Name = "totalTo")]
        public decimal? TotalTo { get; set; }

        [FromQuery(Name = "createdFrom")]
        public DateTime? CreatedFrom { get; set; }

        [FromQuery(Name = "createdTo")]
        public DateTime? CreatedTo { get; set; }
    }
}


