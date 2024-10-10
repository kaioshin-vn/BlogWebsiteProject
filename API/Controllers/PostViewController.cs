using BlogWebsite.Data;
using Data.Database.Table;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostViewController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PostViewController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("id")]
        public async Task<IActionResult> TotalViewPost(Guid id)
        {
            // kiểm tra xem người dùng đã đnhập chưa
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(); // đã đăng nhập đâu
            }
            // chuyển userid thành guid tý use
            var userId = Guid.Parse(userIdClaim.Value);
            // tìm bài post muốn xem
            var postView = await _context.Posts.FindAsync(userId);
            if (postView == null || !postView.IsDeleted)
            {
                return NotFound(); // ko tìm thấy bài viết
            }
            // check xem đã xem post chưa
            var ckeckPostView = await _context.PostViews.FirstOrDefaultAsync(p => p.PostId == id && p.UserId == userId);
            if (ckeckPostView == null)
            {
                // nếu chưa xem => tạo lượt xem
                var viewNew = new PostView() 
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    PostId = id,
                };
                await _context.PostViews.AddAsync(viewNew);
                postView.View += 1;
                _context.Entry(postView).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            return Ok();
        }
    }
}
