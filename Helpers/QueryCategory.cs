using Microsoft.AspNetCore.Mvc;

namespace FluxifyAPI.Helpers
{
    public class QueryCategory : QueryBase
    {

        [FromQuery(Name = "name")]
        public string? Name { get; set; }
        [FromQuery(Name = "description")]
        public string? Description { get; set; }
        [FromQuery(Name = "isActive")]
        public bool? IsActive { get; set; }
    }
}


