using System.ComponentModel.DataAnnotations;

namespace DormMS.DTOs
{
    public class NewsUpdateDto
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Tóm tắt không được để trống")]
        [MaxLength(300)]
        public string? Summary { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống")]
        [MaxLength(500, ErrorMessage = "Nội dung không được quá 500 ký tự")]
        public string Content { get; set; }
        public bool IsImportant { get; set; }
    }
}
