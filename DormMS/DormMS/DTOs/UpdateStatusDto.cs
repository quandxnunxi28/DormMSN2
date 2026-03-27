using System.ComponentModel.DataAnnotations;

namespace DormMS.DTOs
{
    public class UpdateStatusDto
    {
        [Required(ErrorMessage = "Trạng thái không được để trống")]
        public string? Status { get; set; }
    }
}
