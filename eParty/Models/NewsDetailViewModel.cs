using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eParty.Models
{
    public class NewsDetailViewModel
    {
        public News News { get; set; }
        public List<Comment> Comments { get; set; }
        public double AverageRating { get; set; }
        public int TotalComments { get; set; }
    }

    public class AddCommentViewModel
    {
        [Required(ErrorMessage = "News ID là bắt buộc")]
        public int NewsId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn số sao")]
        [Range(1, 5, ErrorMessage = "Số sao phải từ 1 đến 5")]
        public int Stars { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung đánh giá")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Nội dung phải từ 10 đến 1000 ký tự")]
        public string Description { get; set; }
    }
}

