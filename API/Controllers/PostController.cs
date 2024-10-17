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

namespace API.Controllers
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
                postQuery =  postQuery.Where(x => x.Title.Contains(inputSearch)).ToList();
            }
            // Tính tổng số trong
            var totalPages = (int)Math.Ceiling((decimal)postQuery.Count / pageSize);
            // Lấy danh sách phân trang
            var posts =  postQuery
                        .Skip((page - 1) * pageSize) // Bỏ qua số bài viết trên các trang trước
                        .Take(pageSize) // Lấy số bài viết cho trang hiện tại
                        .ToList();

            return Ok(new Pagination<Post>()
            {
                TotalPage = totalPages,
                ListPage = posts
            });
        }

        [HttpGet("getPost")]        
        public async Task<IActionResult> GetAllPost()
        {
            var lstPost = await _context.Posts.Where(x => !x.IsDeleted).ToListAsync();
            return Ok(lstPost);
        }

        [HttpGet("getByIdPost/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var getIdPost = await _context.Posts.FindAsync(id);
            if (getIdPost == null)
            {
                return NotFound();
            }
            return Ok(getIdPost);
        }



        [HttpPost("createPost")]
        public async Task<IActionResult> CreatePostAsync([FromForm] PostDTO _post, IFormFile imgFile)
        {
            if (_post.IdUser == null)
            {
                return BadRequest();
            }
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", imgFile.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await imgFile.CopyToAsync(stream); // Sử dụng CopyToAsync để sao chép tệp
            }
            var post = new Post()
            {
                Id = Guid.NewGuid(),
                IdUser = _post.IdUser,
                Title = _post.Title,
                Description = _post.Description,
                Content = _post.Content,
                CreateDate = DateTime.Now,
                ImgFile = imgFile.FileName
            };
            _context.Posts.Add(post);
             await _context.SaveChangesAsync();
            return Ok(post);    
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
            existingPost.Description = post.Description;
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
    }


    // Lớp này sẽ tạo ở Data cho View dùng nữa
    public class Pagination<T>
    {
        public List<T> ListPage { get; set; }
        public int TotalPage { get; set; }
    }
}
