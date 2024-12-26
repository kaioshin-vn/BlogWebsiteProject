using Microsoft.AspNetCore.Http;
using BlogWebsite.Data;
using Data.Database.Table;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data.DTO.EntitiDTO;
using Data.DTO;
using Blazorise;
using Client.Components.Pages.Restricted;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Html;

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


        [HttpGet("/checkExistedRestriceted/{Word}")]
        public async Task<IActionResult> CheckRS(string Word)
        {
            var existed = _context.RestrictedWords.Any(a => a.Word.ToLower() == Word.ToLower());

            return Ok(existed);
        }

        [HttpGet("/checkExistedTopic/{Topic}")]
        public async Task<IActionResult> CheckTP(string Topic)
        {
            var existed = _context.Topics.Any(a => a.TopicName.ToLower() == Topic.ToLower());

            return Ok(existed);
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


            var lisPostViolence = _context.Posts.Where(a => a.IsDeleted == false && ( a.Content.ToLower().Contains(topic.Word.ToLower()) || a.Title.ToLower().Contains(topic.Word.ToLower()))).ToList();

            var listIdUser = new HashSet<Guid>();

            if (lisPostViolence.Count != 0)
            {
                foreach (var item in lisPostViolence)
                {
                    listIdUser.Add(item.IdUser);
                    var posthide = new PostHideByRestricted();

                    posthide.Id = Guid.NewGuid();

                    posthide.IdPost = item.Id;

                    posthide.IdRestricted = topic.Id;

                    _context.PostHideByRestricted.Add(posthide);
                }
            }
            _context.SaveChanges();


            return Ok(listIdUser);
        }

        async Task<(bool state, string word)> CheckContent(string htmlString, string word)
        {

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlString);

            var content = doc.DocumentNode.InnerText;
            if (content.ToLower().Contains(word.ToLower()))
            {
                return (false, word);
            }
            return (true, "");
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTopic(Guid id, [FromBody] RestrictedWord topic)
        {
            if (id != topic.Id)
            {
                return BadRequest();
            }

            var listPostHide = _context.PostHideByRestricted.Where(a => a.IdRestricted == topic.Id);

            _context.PostHideByRestricted.RemoveRange(listPostHide);
            _context.SaveChanges();




            _context.Entry(topic).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            var lisPostViolence = _context.Posts.Where(a => a.IsDeleted == false && (a.Content.ToLower().Contains(topic.Word.ToLower()) || a.Title.ToLower().Contains(topic.Word.ToLower()))).ToList();

            var listIdUser = new HashSet<Guid>();

            if (lisPostViolence.Count != 0)
            {
                foreach (var item in lisPostViolence)
                {
                    listIdUser.Add(item.IdUser);
                    var posthide = new PostHideByRestricted();

                    posthide.Id = Guid.NewGuid();

                    posthide.IdPost = item.Id;

                    posthide.IdRestricted = topic.Id;

                    _context.PostHideByRestricted.Add(posthide);
                }
            }
            _context.SaveChanges();

            if (!TopicExists(id))
            {
                return NotFound();
            }

            return Ok(listIdUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(Guid id)
        {
            var topic = await _context.RestrictedWords
                .FirstOrDefaultAsync(t => t.Id == id);

            var listPostHide = _context.PostHideByRestricted.Where(a => a.IdRestricted == id);

            _context.PostHideByRestricted.RemoveRange(listPostHide);

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
