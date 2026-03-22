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
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] NewsCreateDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var createBy = int.Parse(userId);
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
            return Ok(new { news.NewsId });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] NewsUpdateDto dto)
        {
            var news = _context.News.FirstOrDefault(x => x.NewsId == id);
            if (news == null) return NotFound();

            news.Title = dto.Title;
            news.Summary = dto.Summary;
            news.Content = dto.Content;
            news.IsImportant = dto.IsImportant;
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var news = _context.News.FirstOrDefault(x => x.NewsId == id);
            if (news == null) return NotFound();

            news.Status = "INACTIVE";
            _context.SaveChanges();
            return Ok();
        }
    }

}