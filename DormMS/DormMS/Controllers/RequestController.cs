using System.Security.Claims;
using DormMS.DTOs;
using DormMS.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace DormMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RequestController : ControllerBase
    {
        private readonly DormMsnContext _context;
        public RequestController(DormMsnContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int pageSize =10, [FromQuery] string? status = null)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = int.Parse(userIdClaim);
            var query = _context.Complaints
                .Where(x => x.UserId == userId)
                .Where(x => string.IsNullOrEmpty(status) || x.Status == status)
                .OrderByDescending(x => x.DateFiled);
            var total = query.Count();
            var totalPages = (int)Math.Ceiling((double)total/pageSize);
            var data = query
                .Skip((page -1) * pageSize)
                .Take(pageSize)
                .Select( x=> new
                {
                    x.ComplaintId,
                    x.Issue,
                    x.Description,
                    x.DateFiled,
                    x.Status,
                    RoomNumber = _context.Rooms
                    .Where(r => r.RoomId == (int?)x.RoomId)
                    .Select(r => r.RoomNumber)
                    .FirstOrDefault()
                }).ToList();
            return Ok(new {data, total, totalPages, currentPage = page});
        }
        [HttpGet("admin")]
        public IActionResult GetAllByAdmin(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string status = null,
            [FromQuery] string issue = null)
        {
            var query = _context.Complaints
                .Where(x => string.IsNullOrEmpty(status) || x.Status == status)
                .Where(x => string.IsNullOrEmpty(issue) || x.Issue == issue)
                .OrderByDescending(x => x.DateFiled);
            var total = query.Count();
            var totalPages = (int)Math.Ceiling((double)total / pageSize);
            var data = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.ComplaintId,
                    x.Issue,
                    x.Description,
                    x.DateFiled,
                    x.Status,
                    UserName = _context.HostelUsers
                    .Where(u => u.UserId == x.UserId)
                    .Select(u => u.Name)
                    .FirstOrDefault(),
                    RoomNumber = _context.Rooms
                    .Where(r => r.RoomId == (int?)x.RoomId)
                    .Select(r => r.RoomNumber)
                    .FirstOrDefault()
                }).ToList();
            return Ok(new { data, total, totalPages, currentPage = page });
        }
        [HttpGet("{id}")]
        public IActionResult GetDetail(int id)
        {
            var data = _context.Complaints
                .Where(x => x.ComplaintId == id)
                .Select(x => new
                {
                    x.ComplaintId,
                    x.Issue,
                    x.Description,
                    x.DateFiled,
                    x.Status,
                    UserName = _context.HostelUsers
                    .Where(u => u.UserId == x.UserId)
                    .Select(u => u.Name)
                    .FirstOrDefault(),
                    RoomNumber = _context.Rooms
                    .Where(r => r.RoomId == (int?)x.RoomId)
                    .Select(r => r.RoomNumber)
                    .FirstOrDefault()
                })
                .FirstOrDefault();
            if (data == null) 
                return BadRequest(new {message = "Không tìm thấy yêu cầu "});
            return Ok(data);
        }
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            var complaint = _context.Complaints.FirstOrDefault(x => x.ComplaintId == id);
            if (complaint == null) return NotFound(new { message = "Không tìm thấy yêu cầu" });
            if (complaint.Status != "Pending")
                return BadRequest(new { message = "Đã xử lý rồi, không thể cập nhật lại" });
            complaint.Status = dto.Status; 
            _context.SaveChanges();
            return Ok(new { message = "Cập nhật trạng thái thành công" });
        }
        [HttpPost]
        public IActionResult Create([FromBody] CreateComplaintDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;            
            var userId = int.Parse(userIdClaim);
            var allotment = _context.Allotments.FirstOrDefault(x => x.UserId == userId && x.Status == "Active");
            var isStudent = _context.HostelUsers.FirstOrDefault(x => x.UserId == userId && x.Role == 2);
            if (isStudent == null)
            {
                return BadRequest(new { message = "Bạn không phải sinh viên! Không thể tạo request" });
            }
            if (allotment == null)
            {
                return BadRequest(new {message = "Bạn chưa được phân phòng"});
            }
            var complaint = new Complaint
            {
                UserId = userId,
                RoomId = allotment.RoomId,
                Issue = dto.Issue,
                DateFiled = DateOnly.FromDateTime(DateTime.Now),
                Description = dto.Description,
                Status = "Pending"
            };
        
            _context.Complaints.Add(complaint);
            _context.SaveChanges();
            return Ok (new { message = "Yêu cầu đã được gửi thành công" });
        }
    }
}
