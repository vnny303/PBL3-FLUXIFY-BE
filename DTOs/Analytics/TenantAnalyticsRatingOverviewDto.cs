namespace FluxifyAPI.DTOs.Analytics
{
    public class TenantAnalyticsRatingOverviewDto
    {
        public DateTime FromUtc { get; set; }
        public DateTime ToUtc { get; set; }
        public int TotalReviews { get; set; }
        public decimal AverageRating { get; set; }
        public int FiveStarCount { get; set; }
        public int FourStarCount { get; set; }
        public int ThreeStarCount { get; set; }
        public int TwoStarCount { get; set; }
        public int OneStarCount { get; set; }
    }
}
