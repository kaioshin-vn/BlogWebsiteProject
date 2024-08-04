using BlogWebsite.Data;
using Data.Database;
using Data.Database.Table;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
       // private ApplicationDbContext _context;
        private List<Post> _lstPost = new List<Post>();
        public PostController()
        {
           // _context = context;

            for (int i = 1; i <= 100; i++)
            {
                _lstPost.Add(new Post
                {
                    Id = Guid.NewGuid(),
                    IdUser = Guid.NewGuid(),
                    Title = "I have a question" + i,
                    NomalizedTitle = "question",
                    CreateDate = DateTime.Now,
                    Content = "What do you favour ite to webside? ",
                    Likes = "10000",
                    View = i,
                    IsDeleted = false,
                    User = null, // Assuming you have the ApplicationUser set up
                    TagPosts = null,
                    PostSaves =null,
                    PaidPosts = null
                });
            }
        }

        [HttpGet("get_all")]
        public IActionResult GetAllPost(int page = 1, string inputSearch = null)
        {
            int pagesize = 10;
            var postResult = _lstPost;
            if (!string.IsNullOrEmpty(inputSearch))
            {
                postResult = postResult.Where(x => x.Title.Contains(inputSearch)).ToList();
            }
            var totalPages = (int)Math.Ceiling((decimal)postResult.Count / pagesize);
            return Ok(new Pagination<Post>()
            {
                TotalPage = totalPages,
                ListPage = postResult.Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList()
            });
        }
    }
    // Lớp này sẽ tạo ở Data cho View dùng nữa
    public class Pagination<T>
    {
        public List<T> ListPage { get; set; }
        public int TotalPage { get; set; }
    }
}
