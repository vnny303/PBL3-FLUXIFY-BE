using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.Helpers
{
    public class QueryReview : QueryBase
    {
        [FromQuery(Name = "rating")]
        [Range(1, 5)]
        public int? Rating { get; set; }
    }
}
