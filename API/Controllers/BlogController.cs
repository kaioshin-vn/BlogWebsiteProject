using BlogWebsite.Data;
using Data.Database.Table;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private ApplicationDbContext _context;
        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("getAllBlog")]
        public async Task<IActionResult> GetAllBlog()
        {
            var lstBlogs = await _context.Categories.ToListAsync();
            return Ok(lstBlogs);
        }

        [HttpGet("getByIdBlog/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var getId = await _context.Categories.FindAsync(id);
            return Ok(getId);
        }

        [HttpPost("createBlog")]
        public async Task<IActionResult> CreateBlog(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return Ok(category);
        }

        //[HttpPut("updateBlog/{id}")]
        //public Task<IActionResult> UpdateBlog(Guid id)
        //{

        //}
    }
}
