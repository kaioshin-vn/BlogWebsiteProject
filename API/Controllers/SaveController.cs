using BlogWebsite.Data;
using Data.Database.Table;
using Data.DTO.EntitiDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaveController : ControllerBase
    {
        ApplicationDbContext _context;
        public SaveController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("get_savepost")]
        public async Task<IActionResult> GetCategoryPost()
        {
            return Ok(await _context.Saves.ToListAsync());
        }

        [HttpPost("create_savepost")]
        public async Task<IActionResult> GetCategoryPost([FromBody] SaveDTO savePost)
        {
            Save save = new Save()
            {
                Id = Guid.NewGuid(),
                IdUser = savePost.IdUser,
                SaveName = savePost.SaveName,
            };
            _context.Saves.Add(save);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("update_savepost/{id}")]
        public async Task<IActionResult> UpdateCategoryPost(Guid id, SaveDTO savePost)
        {
            var getId = await _context.Saves.FindAsync(id);
            if (getId == null)
            {
                return BadRequest();
            }
            getId.SaveName = savePost.SaveName;
            _context.Saves.Update(getId);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("deleteSave/{idSave}/{idUser}")]
        public async Task<IActionResult> DeleteCategoryPost(Guid idSave, Guid idUser)
        {
            var getId = await _context.Saves.FirstOrDefaultAsync(x => x.Id == idSave && x.IdUser == idUser);
            if (getId == null)
            {
                return BadRequest();
            }
            _context.Saves.Remove(getId);
            await _context.SaveChangesAsync();
            return Ok(getId);
        }
    }
}
