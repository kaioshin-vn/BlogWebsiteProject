using Azure.Core;
using BlogWebsite.Data;
using Data.Database.Table;
using Data.DTO;
using Data.DTO.EntitiDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.CommentController
{
    public class CommentController : Controller
    {
        ApplicationDbContext _context;
        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CommentController/Details/5
        [HttpGet("commment/getAllComments/{idPost}")]
        public List<CommentDTO> GetCommentsPost(Guid idPost, [FromBody] List<Guid> listPostExisted)
        {

            return new List<CommentDTO>();
        }

        // POST: CommentController/Create
        [HttpPost("/comment/addNewComment")]
        public async Task Create([FromBody] PostCommentDTO comment)
        {
            var newComment = new Response()
            {
                Id = comment.Id,
                IdPost = comment.IdPost,
                IdUser = comment.IdUser,
                Content = comment.Content,
                CreateDate = DateTime.Now,
            };

            _context.Responses.Add(newComment);
            await _context.SaveChangesAsync();
        }
    }
}
