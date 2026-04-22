using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.DTOs.Review
{
    public class UpdateReviewRequestDto
    {
        [Required(ErrorMessage = "Rating là bắt buộc")]
        [Range(1, 5, ErrorMessage = "Rating phải từ 1 đến 5")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Comment là bắt buộc")]
        [StringLength(2000, ErrorMessage = "Comment tối đa 2000 ký tự")]
        public string Comment { get; set; } = string.Empty;
    }
}
