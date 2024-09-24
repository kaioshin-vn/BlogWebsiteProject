using BlogWebsite.Data;
using Data.Database.Table;
using Data.DTO.EntitiDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostCommentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PostCommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> GetComments(Guid postId)
        {
            var postToComment = await _context.Posts.FindAsync(postId);
            if (postToComment == null || postToComment.IsDeleted)
            {
                return NotFound(); // ko tìm thấy bài viết
            }

            var lstComments = await _context.PostComments
                 .Where(c => c.PostId == postId && c.ParentCommentId == null) // Lấy bình luận gốc
                .Include(c => c.User) // Bao gồm thông tin người dùng
                .Include(c => c.Replies) // Bao gồm các phản hồi
                .ThenInclude(r => r.User) // Bao gồm thông tin người dùng cho phản hồi
                .ToListAsync();

            return Ok(lstComments);
        }

        [HttpPost("{postId}/comments/{commentId}/reply")]
        public async Task<IActionResult> AddComment(Guid postId,Guid commentId, [FromBody] PostCommentDTO postComment)
        {
            // checck xem đã đăng nhập chưa
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            // convert
            var userId = Guid.Parse(userIdClaim.Value);

            // tìm post muốn bình luận
            var postIdComment = await _context.Posts.FindAsync(postId);
            if (postIdComment == null || postIdComment.IsDeleted)
            {
                return NotFound(); // Không tìm thấy
            }

            // tìm bình luận cha
            var parentComment = await _context.PostComments.FindAsync(commentId);
            if (parentComment == null)
            {
                return NotFound(); // ko thấy
            }

           // Tạo mới một đối tượng PostComment
            var newPostComment = new PostComment
            {
                Id = Guid.NewGuid(),
                Content = postComment.Content,
                CreateDate = DateTime.Now,
                PostId = postId,
                UserId = userId,
                ParentCommentId = postIdComment.Id, // gán comment cha
            };
            await _context.PostComments.AddAsync(newPostComment);
            await _context.SaveChangesAsync();
            return Ok();
        }



        [HttpPut("{postId}/comments/{commentId}")]
        public async Task<IActionResult> UpdateComment(Guid postId, Guid commentId, [FromBody] PostCommentDTO updatedComment)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(); //  chưa đăng nhập
            }
            Guid userId = Guid.Parse(userIdClaim.Value);
            var post = await _context.Posts.FindAsync(postId);
            if (post == null || post.IsDeleted)
            {
                return NotFound(); // k thấy bài viết
            }
            var existingComment = await _context.PostComments.FindAsync(commentId);
            if (existingComment == null || existingComment.UserId != userId)
            {
                return NotFound(); // k thấy bình luận hoặc người dùng k phải chủ sở hữu
            }
            // Cập nhật nội dung bình luận
            existingComment.Content = updatedComment.Content;
            existingComment.CreateDate = DateTime.UtcNow;
            _context.PostComments.Update(existingComment);
            await _context.SaveChangesAsync();

            return Ok(existingComment);
        }

        [HttpPut("{commentId}")]
        public async Task<IActionResult> DeleteComment(Guid commentId, PostCommentDTO postComment)
        {
            var getIdComment = await _context.PostComments.FindAsync(commentId);
            if (getIdComment == null)
            {
                return NotFound(); // ko tìm thấy
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || postComment.UserId != Guid.Parse(userIdClaim.Value))
            {
                return Unauthorized(); // Người dùng không có quyền sửa
            }
            getIdComment.Content = postComment.Content;
            getIdComment.CreateDate = DateTime.Now;
            _context.PostComments.Update(getIdComment);
            await _context.SaveChangesAsync();
            return Ok();
        }
        }
    }
