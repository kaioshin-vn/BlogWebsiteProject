using BlogWebsite.Data;
using Data.Database.Table;
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
        [HttpPost("id")]
        public async Task<IActionResult> ToggleLikePost(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(); // đã đăng nhập đâu
            }
            Guid userId = Guid.Parse(userIdClaim.Value);

            //tìm post muốn like
            var post = await _context.Posts.FindAsync(id);
            if (post == null || post.IsDeleted)
            {
                return NotFound(); // Không tìm thấy
            }
            // kiểm tra xem đã like chưa
            var checkLike = await _context.PostLikes.FirstOrDefaultAsync(p => p.PostId == id && p.UserId == userId); // nếu thoả mãn thì đã like rồi 
            if (checkLike != null)
            {
                // đã like mà bị dập phát nữa thì dis_like
                _context.PostLikes.Remove(checkLike);
                post.Like -= 1;
            }
            else
            {
                // nếu chưa thích thì thích thôi, ngại gì vết bẩn
                var postLiked = new PostLike()
                {
                    Id = Guid.NewGuid(),
                    PostId = id,
                    UserId = userId,
                };
                await _context.PostLikes.AddAsync(postLiked);
                post.Like += 1;
            }

            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
