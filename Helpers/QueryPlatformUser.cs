using Microsoft.AspNetCore.Mvc;

namespace FluxifyAPI.Helpers
{
    public class QueryPlatformUser : QueryBase
    {
        [FromQuery(Name = "email")]
        public string? Email { get; set; }

        [FromQuery(Name = "fullName")]
        public string? FullName { get; set; }

        [FromQuery(Name = "role")]
        public string? Role { get; set; }

        [FromQuery(Name = "isActive")]
        public bool? IsActive { get; set; }

        [FromQuery(Name = "phone")]
        public string? Phone { get; set; }

        [FromQuery(Name = "createdFrom")]
        public DateTime? CreatedFrom { get; set; }

        [FromQuery(Name = "createdTo")]
        public DateTime? CreatedTo { get; set; }
    }
}
