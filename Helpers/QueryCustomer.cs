using Microsoft.AspNetCore.Mvc;

namespace FluxifyAPI.Helpers
{
    public class QueryCustomer : QueryBase
    {
        [FromQuery(Name = "email")]
        public string? Email { get; set; }

        [FromQuery(Name = "isActive")]
        public bool? IsActive { get; set; }

        [FromQuery(Name = "createdFrom")]
        public DateTime? CreatedFrom { get; set; }

        [FromQuery(Name = "createdTo")]
        public DateTime? CreatedTo { get; set; }
    }
}


