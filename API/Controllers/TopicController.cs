using Microsoft.AspNetCore.Http;
using BlogWebsite.Data;
using Data.Database.Table;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TopicController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("get-all-topics")]
        public async Task<IActionResult> GetAllTopic()
        {
            var listTopic = await _context.Topics.ToListAsync();
            return Ok(listTopic);
        }
    }
}
