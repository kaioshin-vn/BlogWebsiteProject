using BlogWebsite.Data;
using Data.Database.Table;
using Data.DTO.EntitiDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostLikeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PostLikeController( ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPut("/updateLike/{id}")]
        public async Task<IActionResult> ToggleLikePost(Guid id, PostDTO _post)
        {
            if (id == null)
            {
                return BadRequest();
            }
            //tìm post muốn like
            var post = await _context.Posts.FindAsync(_post.Id);

            if (post == null)
            {
                return NotFound(); // Không tìm thấy
            }
            // kiểm tra xem đã like chưa
            var userLike = await _context.PostLikes.FirstOrDefaultAsync(p => p.PostId == _post.Id && p.UserId == _post.IdUser); // nếu thoả mãn thì đã like rồi 
            if (userLike != null)
            {
                // đã like
                _context.PostLikes.Remove(userLike);
                post.Like -=  1;
            }
            else
            {
                // nếu chưa thích thì thích thôi
                var postLiked = new PostLike()
                {
                    Id = Guid.NewGuid(),
                    PostId = id,
                    UserId = _post.IdUser,   // chỗ này hơi sai sai
                };
                await _context.PostLikes.AddAsync(postLiked);
                post.Like += 1;
            }

            _context.Entry(post).State = EntityState.Modified; // update
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
