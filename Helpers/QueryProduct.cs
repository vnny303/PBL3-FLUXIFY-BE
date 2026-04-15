using Microsoft.AspNetCore.Mvc;

namespace FluxifyAPI.Helpers
{
    public class QueryProduct : QueryBase
    {

        [FromQuery(Name = "name")]
        public string? Name { get; set; }

        [FromQuery(Name = "hasAttributes")]
        public bool? HasAttributes { get; set; }
    }
}


