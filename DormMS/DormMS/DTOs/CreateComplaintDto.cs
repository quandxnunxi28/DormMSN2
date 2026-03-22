using System.ComponentModel.DataAnnotations;

namespace DormMS.DTOs
{
    public class CreateComplaintDto
    {
        [Required(ErrorMessage = "Vui lòng chọn loại sự cố")]
        public string Issue { get; set; }

        [MaxLength(500)]
        [Required(ErrorMessage = "Mô tả không được để trống")]
        public string? Description { get; set; }
    }
}
