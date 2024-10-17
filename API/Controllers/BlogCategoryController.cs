using BlogWebsite.Data;
using Data.Database.Table;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogCategoryController : ControllerBase
    {
        private ApplicationDbContext _context;
        public BlogCategoryController(ApplicationDbContext context)
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

        [HttpPost("createCategory")]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return Ok(category);
        }

        [HttpPost("createBlog")]
        public async Task<IActionResult> CreateBlog(Category category)
        {
            //var getParentCategory = await _context.Categories.FindAsync(category.Id);
            var newCategory = new Category()
            {
                Id = Guid.NewGuid(),
                Title = category.Title,
                Content = category.Content,
                Slug = category.Slug,
                ParentCategoryId = category.Id,
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();
            return Ok(category);
        }

        [HttpPut("updateBlog/{id}")]
        public async Task<IActionResult> EditCategory(Guid id, Category category)
        {
            var getId = await _context.Categories.FindAsync(id);
            if(getId == null) {
                return NotFound();
            }
            _context.Update(category);
            await _context.SaveChangesAsync();
            return Ok(getId);

        }

    }
}
