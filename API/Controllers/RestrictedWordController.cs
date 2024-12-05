using Microsoft.AspNetCore.Http;
using BlogWebsite.Data;
using Data.Database.Table;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data.DTO.EntitiDTO;
using Data.DTO;
using Blazorise;
using Client.Components.Pages.Restricted;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestrictedWordController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public RestrictedWordController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("get-all-st")]
        public async Task<IActionResult> GetAllTopic()
        {
            var listTopic = await _context.RestrictedWords.ToListAsync();
            return Ok(listTopic);
        }
        [HttpGet("get-st")]
        public async Task<IActionResult> GetTopics([FromQuery] string searchString = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.RestrictedWords.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(t => t.Word.Contains(searchString));
            }

            var totalCount = await query.CountAsync();
            var topics = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                Topics = topics,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return Ok(result);
        }

        [HttpGet("/getAllRestrictedWord")]
        public async Task<IActionResult> GetRestricted()
        {
            var restrictedWords = _context.RestrictedWords.Select(a => a.Word).ToList();
            return Ok(restrictedWords);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTopic(Guid id)
        {
            var topic = await _context.RestrictedWords.FindAsync(id);

            if (topic == null)
            {
                return NotFound();
            }

            return Ok(topic);
        }

		

        [HttpPost]
        public async Task<IActionResult> CreateTopic([FromBody] RestrictedWord topic)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            topic.Id = Guid.NewGuid();
            _context.RestrictedWords.Add(topic);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTopic), new { id = topic.Id }, topic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTopic(Guid id, [FromBody] RestrictedWord topic)
        {
            if (id != topic.Id)
            {
                return BadRequest();
            }

            _context.Entry(topic).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TopicExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(Guid id)
        {
            var topic = await _context.RestrictedWords
                .FirstOrDefaultAsync(t => t.Id == id);

            try
            {
                _context.RestrictedWords.Remove(topic);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Có lỗi xảy ra khi xóa topic.");
            }
        }

        private bool TopicExists(Guid id)
        {
            return _context.RestrictedWords.Any(e => e.Id == id);
        }
    }
}
