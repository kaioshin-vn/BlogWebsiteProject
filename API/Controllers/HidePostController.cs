using BlogWebsite.Data;
using Data.Database.Table;
using Data.DTO;
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

        [HttpGet("/getListHidePost/{IdUser}")]
        public async Task<IActionResult> GetHidePost(Guid IdUser)
        {
            var listPh = new List<PostHideByRestrictedDTO>();
            var hp = _context.PostHideByRestricted.Include(a => a.RestrictedWord).Include(a => a.Post).ThenInclude(a => a.User).Where(a => a.Post.IsDeleted == false && a.Post.User.Id == IdUser).ToList();

            foreach (var item in hp)
            {
                if (listPh.Any(a => a.IdPost != item.IdPost) || listPh.Count == 0)
                {
                    var phDTO = new PostHideByRestrictedDTO();
                    phDTO.IdPost = item.IdPost;
                    phDTO.Post = item.Post;
                    phDTO.IdRestricted = item.IdRestricted;
                    phDTO.Id = item.Id;
                    phDTO.ListRestrictedWord = string.Join(", ", hp.Select(a => a.RestrictedWord.Word).ToList()) + " .";


                    listPh.Add(phDTO);
                }
            }

            

            return Ok(listPh);
        }
    }
}
