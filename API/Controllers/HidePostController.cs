using BlogWebsite.Data;
using Data.Database.Table;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class HidePostController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HidePostController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost("/addhidepost")]
        public async Task AddHidePost([FromBody] PostHide ph)
        {
            _context.PostHides.Add(ph);
            await _context.SaveChangesAsync();
        }

        [HttpDelete("/deletehidepost/{IdPost}/{IdUser}")]
        public async Task DeleteHidePost(Guid IdPost, Guid IdUser)
        {
            var hp = await _context.PostHides.FirstOrDefaultAsync(a => a.IdPost == IdPost && a.IdUser == IdUser);
            _context.PostHides.Remove(hp);
            await _context.SaveChangesAsync();
        }
    }
}
