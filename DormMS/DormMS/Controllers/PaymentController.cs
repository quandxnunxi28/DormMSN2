using System.Security.Claims;
using DormMS.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
namespace DormMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PaymentController : ControllerBase
    {
        private readonly DormMsnContext _context;
        public PaymentController(DormMsnContext context)
        {
            _context = context;
        }
        [HttpGet]
        public  IActionResult GetAll(
                [FromQuery] int page = 1, 
                [FromQuery] int pageSize = 10,
                [FromQuery] string? status = null)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = int.Parse(userIdClaim);
            var query = _context.Payments
                .Where(x => x.UserId == userId)
                .Where(x => string.IsNullOrEmpty(status) || x.Status == status)
                .OrderByDescending(x => x.Date);
            var total = query.Count();
            var totalPages = (int)Math.Ceiling((double)total / pageSize);
            var data = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.PaymentId,
                    x.Amount,
                    x.Date,
                    x.Method,
                    x.Description,
                    x.Status
                }).ToList();
            return Ok(new { data, total, totalPages, currentPage = page });
        }
    }

    //[HttpGet]
    //    [Authorize]
    //    public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? status = null)
    //    {
    //        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    //        if (userIdClaim == null) return Unauthorized();
    //        var userId = int.Parse(userIdClaim);

    //        var query = _context.Payments
    //            .Where(x => x.UserId == userId)
    //            .Where(x => string.IsNullOrEmpty(status) || x.Status == status)
    //            .OrderByDescending(x => x.Date);

    //        var total = query.Count();
    //        var totalPages = (int)Math.Ceiling((double)total / pageSize);

    //        var data = query
    //            .Skip((page - 1) * pageSize)
    //            .Take(pageSize)
    //            .Select(x => new
    //            {
    //                x.PaymentId,
    //                x.Amount,
    //                x.Date,
    //                x.Method,
    //                x.Description,
    //                x.Status
    //            })
    //            .ToList();

    //        return Ok(new { data, total, totalPages, currentPage = page });
    //    }
    }
