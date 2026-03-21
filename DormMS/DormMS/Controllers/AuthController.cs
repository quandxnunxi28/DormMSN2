using DormMS.Models;
using DormMS.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DormMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private static Dictionary<string, string> otpStorage = new();
        private static Dictionary<string, RegisterDto> registerStorage = new();
        private readonly DormMsnContext _context;
        private readonly IConfiguration _config;
        
        public AuthController(DormMsnContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Không nhận được dữ liệu");

                TryValidateModel(dto);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // ❌ Email đã tồn tại
                if (_context.HostelUsers.Any(x => x.Email == dto.Email))
                {
                    return BadRequest(new
                    {
                        field = "email",
                        message = "Email đã tồn tại"
                    });
                }

                // ❌ Password yếu
                if (!IsStrongPassword(dto.Password))
                {
                    return BadRequest(new
                    {
                        field = "password",
                        message = "Mật khẩu không đủ mạnh"
                    });
                }

                // =============================
                // ✅ CHỈ GỬI OTP - KHÔNG TẠO USER
                // =============================

                var otp = new Random().Next(100000, 999999).ToString();

                otpStorage[dto.Email] = otp;
                registerStorage[dto.Email] = dto;

                var emailService = new EmailService();
                emailService.SendOtp(dto.Email, otp);

                return Ok(new
                {
                    message = "OTP đã gửi về email",
                    email = dto.Email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
        private bool IsStrongPassword(string password)
        {
            if (password.Length < 8)
                return false;

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasUpper && hasLower && hasSpecial;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody]  LoginDto dto)
        {
            var user = _context.HostelUsers
                .Include(u => u.RoleNavigation)
                .FirstOrDefault(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                return Unauthorized(new
                {
                    field = "login",
                    message = "Sai email hoặc mật khẩu"
                });

            var token = GenerateJwt(user);

            return Ok(new
            {
                token,
                fullName = user.Name,
                role = user.RoleNavigation.RoleName
            });
        }

        [HttpPost("verify-otp")]
        public IActionResult VerifyOtp([FromBody] OtpDto dto)
        {
            if (!otpStorage.ContainsKey(dto.Email))
                return BadRequest(new { message = "Chưa gửi OTP" });

            if (otpStorage[dto.Email] != dto.Otp)
                return BadRequest(new { message = "OTP không đúng" });

            // =============================
            // 🔥 CASE 1: REGISTER
            // =============================
            if (registerStorage.ContainsKey(dto.Email))
            {
                var reg = registerStorage[dto.Email];

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(reg.Password);

                var user = new HostelUser
                {
                    Username = reg.Username,
                    Name = reg.Username,
                    Email = reg.Email,
                    Password = hashedPassword,
                    Role = 2,
                    Status = "Active"
                };

                _context.HostelUsers.Add(user);
                _context.SaveChanges();

                registerStorage.Remove(dto.Email);
            }

            // =============================
            // 🔥 LOGIN (google hoặc otp)
            // =============================
            var userDb = _context.HostelUsers
                .Include(x => x.RoleNavigation)
                .FirstOrDefault(x => x.Email == dto.Email);

            var token = GenerateJwt(userDb);

            otpStorage.Remove(dto.Email);

            return Ok(new
            {
                token,
                fullName = userDb.Name
            });
        }
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = "https://localhost:7088/api/auth/google-response";
            var props = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(props, "Google");
        }
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync("Cookies");

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
                return BadRequest("Không lấy được email");

            // 🔍 kiểm tra user trong DB
            var user = _context.HostelUsers
                .Include(x => x.RoleNavigation)
                .FirstOrDefault(x => x.Email == email);

            // ===============================
            // ❌ CHƯA CÓ USER → gửi OTP
            // ===============================
            if (user == null)
            {
                var otp = new Random().Next(100000, 999999).ToString();
                otpStorage[email] = otp;

                var emailService = new EmailService();
                emailService.SendOtp(email, otp);

                // 👉 chuyển sang OTP page
                return Redirect($"/otp.html?email={email}&newUser=true");
            }

            // ===============================
            // ✅ ĐÃ CÓ USER → LOGIN LUÔN
            // ===============================
            var token = GenerateJwt(user);

            return Redirect($"/student.html?token={token}");
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
           

            return Ok(new { message = "Đăng xuất thành công" });
        }


        [Authorize]
        [HttpGet("me")]
        public IActionResult GetMe()
        {
            var username = User.Identity?.Name;

            if (username == null)
                return Unauthorized();

            var user = _context.HostelUsers
                .FirstOrDefault(x => x.Username == username);

            if (user == null)
                return NotFound();

            return Ok(new
            {
                name = user.Name,
                email = user.Email,
                phone = user.Phone,
                gender = user.Gender,
                dob = user.Dob,
                balance = 0
            });
        }

        private string GenerateJwt(HostelUser user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.RoleNavigation.RoleName)
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
