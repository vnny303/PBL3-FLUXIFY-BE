using Microsoft.AspNetCore.Mvc;

namespace FluxifyAPI.Helpers
{
    public class QueryCart : QueryBase
    {
        [FromQuery(Name = "tenantId")]
        public Guid? TenantId { get; set; }

        [FromQuery(Name = "customerId")]
        public Guid? CustomerId { get; set; }
    }
}
