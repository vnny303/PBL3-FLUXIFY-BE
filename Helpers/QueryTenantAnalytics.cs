using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.Helpers
{
    public class QueryTenantAnalytics
    {
        private int _take = 10;

        [FromQuery(Name = "from")]
        public DateTime? From { get; set; }

        [FromQuery(Name = "to")]
        public DateTime? To { get; set; }

        [FromQuery(Name = "take")]
        [Range(1, 100)]
        public int Take
        {
            get => _take;
            set => _take = value <= 0 ? 10 : (value > 100 ? 100 : value);
        }
    }
}
