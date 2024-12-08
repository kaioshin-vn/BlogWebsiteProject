using Microsoft.AspNetCore.Http;
using BlogWebsite.Data;
using Data.Database.Table;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data.DTO.EntitiDTO;
using Data.DTO;
using Blazorise;

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
        [HttpGet("get-topics")]
        public async Task<IActionResult> GetTopics([FromQuery] string searchString = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Topics.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(t => t.TopicName.Contains(searchString));
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTopic(Guid id)
        {
            var topic = await _context.Topics.FindAsync(id);

            if (topic == null)
            {
                return NotFound();
            }

            return Ok(topic);
        }

		[HttpGet("/GetGroupTopic")]
		public async Task<IActionResult> GetGroupTopic()
		{
            var listTopic = new List<Topic>();
            if (_context.Topics.Count() != 0)
            {
                listTopic = _context.Topics.Include(a => a.GroupTopics).ThenInclude(a => a.Group).ThenInclude(a => a.MemberGroups).ToList();
                foreach (var item in listTopic)
                {
                    item.GroupTopics = item.GroupTopics.Where(a => a.Group.isDeleted == false).ToList();
                }
            }

			return Ok(listTopic);
		}

        [HttpGet("/AllGroupTopic/{IdTopic}")]
        public async Task<IActionResult> AllGroupTopic(Guid IdTopic)
        {
            var topic = new TopicDTO();

            if (_context.GroupTopics.Count() != 0)
            {
                var tp = await _context.Topics.FirstOrDefaultAsync(a => a.IdTopic == IdTopic);
                topic.Id = tp.IdTopic;
                topic.Name = tp.TopicName;
            }

            var listIdGroup = new List<Guid>();
            if (_context.GroupTopics.Count() != 0)
            {
                listIdGroup = _context.GroupTopics.Where(a => a.IdTopic == IdTopic).Select(a => a.IdGroup).ToList();
            }

            var listGroup = new List<GroupDTO>();

            listGroup = _context.Groups.Where(a => listIdGroup.Contains(a.IdGroup)).Select(g => new GroupDTO
            {
                IdGroup = g.IdGroup,
                Name = g.Name,
                Description = g.Description,
                ImgGroup = g.ImgGroup == null ? "/img/icon.jpg" : g.ImgGroup,
                StateGroup = g.StateGroup,
                MemberCount = _context.MemberGroups.Count(m => m.IdGroup == g.IdGroup)
            }).ToList();

            topic.groupDTOs = listGroup;

            return Ok(topic);
        }


        [HttpGet("/SearchAllGroupTopic/{TopicName}")]
        public async Task<IActionResult> SearchAllGroupTopic(string TopicName)
        {
            var topic = new TopicDTO();

            if (_context.GroupTopics.Count() != 0)
            {
                var tp = await _context.Topics.FirstOrDefaultAsync(a => a.TopicName.ToLower().Contains((TopicName.ToLower())));
                topic.Id = tp.IdTopic;
                topic.Name = tp.TopicName;
            }

            var listIdGroup = new List<Guid>();
            if (_context.GroupTopics.Count() != 0)
            {
                listIdGroup = _context.GroupTopics.Where(a => a.IdTopic == topic.Id).Select(a => a.IdGroup).ToList();
            }

            var listGroup = new List<GroupDTO>();

            listGroup = _context.Groups.Where(a => listIdGroup.Contains(a.IdGroup)).Select(g => new GroupDTO
            {
                IdGroup = g.IdGroup,
                Name = g.Name,
                Description = g.Description,
                ImgGroup = g.ImgGroup == null ? "/img/icon.jpg" : g.ImgGroup,
                StateGroup = g.StateGroup,
                MemberCount = _context.MemberGroups.Count(m => m.IdGroup == g.IdGroup)
            }).ToList();

            topic.groupDTOs = listGroup;

            return Ok(topic);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTopic([FromBody] Topic topic)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            topic.IdTopic = Guid.NewGuid();
            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTopic), new { id = topic.IdTopic }, topic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTopic(Guid id, [FromBody] Topic topic)
        {
            if (id != topic.IdTopic)
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
            var topic = await _context.Topics
                .Include(t => t.GroupTopics)
                .FirstOrDefaultAsync(t => t.IdTopic == id);

            if (topic == null)
            {
                return NotFound("Không tìm thấy Topic.");
            }

            if (topic.GroupTopics != null && topic.GroupTopics.Any())
            {
                return BadRequest("Topic đang nằm trong Group vui lòng kiểm tra lại.");
            }

            try
            {
                _context.Topics.Remove(topic);
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
            return _context.Topics.Any(e => e.IdTopic == id);
        }
    }
}
