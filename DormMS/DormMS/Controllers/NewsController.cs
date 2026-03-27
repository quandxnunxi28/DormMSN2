using DormMS.DTOs;
using DormMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace DormMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class NewsController : ControllerBase
    {
        private readonly DormMsnContext _context;
        public NewsController(DormMsnContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] int page = 1,
                                    [FromQuery] int pageSize = 6, 
                                    [FromQuery] string? search = null)
        {
            var query = _context.News
                .Where(x => x.Status == "ACTIVE")
                .Where(x => string.IsNullOrEmpty(search) || x.Title.Contains(search))
                .OrderByDescending(x => x.IsImportant)
                .ThenByDescending(x => x.CreatedDate);

            var totalRecords = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var data = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.NewsId,
                    x.Title,
                    x.Summary,
                    x.IsImportant,
                    x.CreatedDate,
                    CreatedByName = _context.HostelUsers
                        .Where(u => u.UserId == x.CreatedBy)
                        .Select(u => u.Name)
                        .FirstOrDefault()
                })
                .ToList();

            return Ok(new
            {
                data,
                totalRecords,
                totalPages,
                currentPage = page
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetDetail(int id)
        {
            var data = _context.News
                .Where(x => x.NewsId == id)
                .Select(x => new
                {
                    x.NewsId,
                    x.Title,
                    x.Summary,
                    x.Content,
                    x.IsImportant,
                    x.CreatedDate,
                    CreatedByName = _context.HostelUsers
                        .Where(u => u.UserId == x.CreatedBy)
                        .Select(u => u.Name)
                        .FirstOrDefault()
                })
                .FirstOrDefault();
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [Authorize( Roles = "Admin")]
        public IActionResult Create([FromBody] NewsCreateDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dữ liệu không hợp lệ");

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("Không xác định được user");

                if (!int.TryParse(userId, out int createBy))
                    return BadRequest("UserId không hợp lệ");

                var news = new News
                {
                    Title = dto.Title,
                    Summary = dto.Summary,
                    Content = dto.Content,
                    IsImportant = dto.IsImportant,
                    Status = "ACTIVE",
                    CreatedBy = createBy,
                    CreatedDate = DateTime.Now,
                    Type = "GENERAL"
                };

                _context.News.Add(news);
                _context.SaveChanges();

                return Ok(new { news.NewsId , message = "Tạo thông báo thành công!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);

                return StatusCode(500, new
                {
                    message = "Lỗi server",
                    error = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize( Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] NewsUpdateDto dto)
        {
            var news = _context.News.FirstOrDefault(x => x.NewsId == id);
            if (news == null) 
                return BadRequest(new {message = "Thông báo không tồn tại"});

            news.Title = dto.Title;
            news.Summary = dto.Summary;
            news.Content = dto.Content;
            news.IsImportant = dto.IsImportant;
            _context.SaveChanges();
            return Ok(new { message = "Cập nhật thông báo thành công!" });
        }

        [HttpDelete("{id}")]
        [Authorize( Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var news = _context.News.FirstOrDefault(x => x.NewsId == id);
            if (news == null) return NotFound();

            news.Status = "INACTIVE";
            _context.SaveChanges();
            return Ok(new { message = "Xóa thông báo thành công!" });
        }
    }

}