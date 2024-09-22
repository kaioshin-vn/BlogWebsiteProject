using API.StaticClass;
using BlogWebsite.Data;
using Data.Database;
using Data.Database.Table;
using Data.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private ApplicationDbContext _context;
        private List<Post> _lstPost = new List<Post>();
        public PostController(ApplicationDbContext context)
        {
            _context = context;

            for (int i = 1; i <= 103; i++)
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
                    PostSaves = null,
                    PaidPosts = null
                });
            }
        }

        [HttpGet("get-list-post")]
        public IActionResult GetObject(int page = 1, string inputSearch = null)
        {
            var listPost = _context.Posts.ToList();

            if (!string.IsNullOrEmpty(inputSearch))
            {

                listPost = listPost.Where(x => x.Title.Contains(inputSearch)).ToList();
            }

            return Ok(_lstPost.GetObject(page));
        }
    }
}
