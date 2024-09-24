using BlogWebsite.Data;
using Data.Database;
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
    public class PostController : ControllerBase
    {
        ApplicationDbContext _context;

        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        public PostController(ApplicationDbContext context) 
        {
            _context = context;

            // Tạo thư mục lưu trữ nếu chưa tồn tại
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        [HttpGet("get_all")]
        public IActionResult GetAllPost(int page = 1, string inputSearch = null)
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

        [HttpPost("create")]
        public async Task<IActionResult> CreatePostAsync([FromBody] PostDTO _post, IFormFile image)
        {
            // claim check
            // lấy IdUser của người claim của người đã đăng nhập
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(); // người dùng chưa đăng nhập
            }
            //chuyển chuổi thành Guid
            var userId = Guid.Parse(userIdClaim.Value);

            if (image != null && image.Length > 0)
            {
                // Kiểm tra định dạng tệp
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(image.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest("Định dạng tệp không hợp lệ. Chỉ cho phép .jpg, .jpeg, .png, .gif");
                }

                var fileName = Path.GetFileNameWithoutExtension(image.FileName);
                var newFileName = $"{fileName}_{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(_storagePath, newFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                _post.ImagePath = $"Uploads/{newFileName}";
            }


            //add
            var post = new Post()
            {
                Id = Guid.NewGuid(),
                IdUser = userId,
                Title = _post.Title,
                NomalizedTitle = _post.NomalizedTitle, // định danhh nghĩa hoá
                CreateDate = DateTime.Now,
                Content = _post.Content,
            };
            _context.Posts.Add(post);
             await _context.SaveChangesAsync();
            return Ok(post);   
        }


        [HttpPut("updateId")]
        public async Task<IActionResult> EditPost(Guid id, PostDTO post)
        {
            //lấy id người dùng đã đăng nhập
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); 
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            // chuyển thành guid
            var userId = Guid.Parse(userIdClaim.Value);
            var existingPost = _context.Posts.FirstOrDefault(p => p.Id == id && p.IdUser == userId);
            if (existingPost == null || !existingPost.IsDeleted)
            {
                return NotFound(); // không tìm thầy bài viết
            }
            existingPost.Title = post.Title;
            existingPost.NomalizedTitle = post.NomalizedTitle;
            existingPost.CreateDate = DateTime.Now;
            existingPost.Content = post.Content;
            _context.Posts.Update(existingPost);
            _context.SaveChanges();
            return Ok(existingPost);
        }

        [HttpPut("deleteId")]        
        public IActionResult DeletePost(Guid id)
        {
            //lấy id người dùng đã đăng nhập
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            // chuyển thành guid
            var userId = Guid.Parse(userIdClaim.Value);
            var deletePost = _context.Posts.FirstOrDefault(p => p.Id == id && p.IdUser == userId);
            if (deletePost == null)
            {
                return NotFound();
            }
            deletePost.IsDeleted = true;
            _context.Posts.Update(deletePost);
            _context.SaveChanges();
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
