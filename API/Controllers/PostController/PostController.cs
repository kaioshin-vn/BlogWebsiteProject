using API.StaticClass;
using BlogWebsite.Data;
using Data.Database;
using Data.Database.Table;
using Data.DTO.EntitiDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.WebSockets;
using System.Security.Claims;

namespace API.Controllers.PostController
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public PostController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment; 
        }

        [HttpGet("getPostPagination")]
        public IActionResult GetAllPostPage(int page = 1, string inputSearch = null)
        {
            int pageSize = 10;
            // Truy vấn csdl
            var postQuery = _context.Posts.Where(p => !p.IsDeleted).ToList(); // lst ko chứa post đã xoá

            // Tìm kiếm theo tiêu đề
            if (!string.IsNullOrEmpty(inputSearch))
            {
                postQuery = postQuery.Where(x => x.Title.Contains(inputSearch)).ToList();
            }
            // Tính tổng số trong
            var totalPages = (int)Math.Ceiling((decimal)postQuery.Count / pageSize);
            // Lấy danh sách phân trang
            var posts = postQuery
                        .Skip((page - 1) * pageSize) // Bỏ qua số bài viết trên các trang trước
                        .Take(pageSize) // Lấy số bài viết cho trang hiện tại
                        .ToList();

            return Ok(new Pagination<Post>()
            {
                TotalPage = totalPages,
                ListPage = posts
            });
        }

        [HttpGet("/GetListPostIntro/{idUser}")]
        public async Task<List<PostIntroDTO>> GetListPostIntro (Guid idUser)
        {
            var listPost = await _context.Posts.Where(a => a.IsDeleted == false ).ToListAsync();

            var listIntroPost = new List<PostIntroDTO>();
            foreach (var item in listPost)
            {
                var introPost = await GetPostIntro(item);
                listIntroPost.Add(introPost);
            }
            return listIntroPost;
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


        [HttpPost("/createPost")]
        public async Task<IActionResult> CreatePostAsync([FromBody] PostDTO _post)
        {
            if (_post.IdUser == null)
            {
                return BadRequest();
            }
            var post = new Post()
            {
                Id = _post.Id,
                IdUser = _post.IdUser,
                Title = _post.Title,
                Content = _post.Content,
                CreateDate = DateTime.Now,
                VideoFile = _post.VideoFile,
                ImgFile = _post.ImgFile,
                Like = ""
            };
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
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

        [HttpPut("updatePost/{idPost}")]
        public async Task<IActionResult> EditPost(Guid idPost, PostDTO post)
        {

            var existingPost = await _context.Posts.FirstOrDefaultAsync(p => p.Id == idPost && p.IdUser == post.IdUser);
            if (existingPost == null)
            {
                return NotFound(); // không tìm thầy bài viết
            }
            existingPost.Title = post.Title;
            existingPost.Content = post.Content;
            existingPost.EditDate = DateTime.Now;
            existingPost.ImgFile = post.ImgFile;

            _context.Posts.Update(existingPost);
            await _context.SaveChangesAsync();
            return Ok(existingPost);
        }

        [HttpPut("deletePost/{idPost}")]
        public async Task<IActionResult> DeletePost(Guid idPost, PostDTO post)
        {
            var deletePost = _context.Posts.FirstOrDefault(p => p.Id == idPost && p.IdUser == post.IdUser);
            if (deletePost == null)
            {
                return NotFound();
            }
            deletePost.IsDeleted = true;
            _context.Posts.Update(deletePost);
            await _context.SaveChangesAsync();
            return Ok();
        }
        private async Task<PostIntroDTO> GetPostIntro(Post post)
        {
            var introPost = post.GetIntroPost();
            var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == post.IdUser);
            var commment =  _context.Responses.Where(a => a.IdPost == post.Id);
            var replyCount = 0;
            var replyComment = _context.ReplyResponses.Where(a => commment.Any(b => b.Id == a.IdResponse) );

            var totalReply = commment.Count() + replyComment.Count();

            introPost.CommentCount = totalReply;
            introPost.UserName = user.FullName;
            introPost.Avatar = user.Img;
            return introPost;
        }

    }


    // Lớp này sẽ tạo ở Data cho View dùng nữa
    public class Pagination<T>
    {
        public List<T> ListPage { get; set; }
        public int TotalPage { get; set; }
    }
}
