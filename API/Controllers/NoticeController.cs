using BlogWebsite.Data;
using Client.VNPayService;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public class NoticeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NoticeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("getAmountNoticeUser/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var noticeCount = _context.Notices.Where( a => a.ToUser == id && a.isSeen == false).Count();
            return Ok(noticeCount);
        }
    }
}
