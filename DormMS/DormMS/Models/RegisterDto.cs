using System.ComponentModel.DataAnnotations;

namespace DormMS.Models
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username không được để trống")]
        public string Username { get; set; } = null!;



        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = null!;



        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; } = null!;
    }
}
