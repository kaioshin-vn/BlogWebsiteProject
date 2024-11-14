using BlogWebsite.Data;
using Data.Database.Table;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostSaveController : ControllerBase
    {
        ApplicationDbContext _context;
        public PostSaveController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetPostSave()
        {
            return Ok(await _context.PostSaves.ToListAsync());
        }
       
        [HttpGet("getPostInSave/{idSave}")]
        public async Task<IActionResult> GetPostInSave(Guid idSave)
        {
            if (idSave == null)
            {
                return BadRequest();
            }
            var post = await _context.PostSaves
                .Where(p => p.IdSave == idSave)
                .Include(p => p.Post)
                .Select(p => p.Post)
                .ToListAsync();
            if (post == null)
            {
                return NotFound("No post in this category");
            }
            return Ok(post);
        }

        [HttpPost("addPostSave/{idPost}/{idSave}")]
        public async Task<IActionResult> AddPostSave(Guid idPost, Guid idSave)
        {
            var getIdPost = await _context.Posts.FindAsync(idPost);
            if (getIdPost == null) return BadRequest();
            var getIdSave = await _context.Saves.FindAsync(idSave);
            if (getIdSave == null) return BadRequest();

            var postSave = new PostSave()
            {
                IdPost = idPost,
                IdSave = idSave
            };
            _context.PostSaves.Add(postSave);
            await _context.SaveChangesAsync();

            return Ok(postSave);
        }

        [HttpDelete("deletePostInSave/{idSave}/{idPost}")]
        public async Task<IActionResult> DeletePostInSave(Guid idSave, Guid idPost)
        {
            var getId = await _context.PostSaves.FirstOrDefaultAsync(x => x.IdSave == idSave && x.IdPost == idPost);
            if (getId == null)
            {
                return BadRequest();
            }
            _context.PostSaves.Remove(getId);
            await _context.SaveChangesAsync();
            return Ok(getId);
        }
    }
}
