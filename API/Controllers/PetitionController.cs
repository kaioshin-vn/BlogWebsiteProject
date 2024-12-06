using BlogWebsite.Data;
using Data.Database.Table;
using Data.DTO.EntitiDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    public class PetitionController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PetitionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("/petition/add")]
        public async Task GetAllTopic([FromBody] Petition petition)
        {
            await _context.Petitions.AddAsync(petition);
            _context.SaveChanges();
            Ok();
        }
        [HttpGet("/getpetition/{Id}")]
        public async Task<IActionResult> GetTopics(Guid Id)
        {
            var result = await _context.Petitions.FirstOrDefaultAsync(petition => petition.Id == Id);
            return Ok(result);
        }

        [HttpPut("/sendpetition/{Id}")]
        public async Task<IActionResult> SendPetition(Guid Id, [FromBody] Petition petition)
        {
            _context.Petitions.Update(petition);
            _context.SaveChanges();
            return Ok();
        }

        [HttpGet("/getPetitionDetail/{Id}")]
        public async Task<IActionResult> GetPetitionDetail(Guid Id)
        {
            var petition = await _context.Petitions.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == Id);
            _context.SaveChanges();
            return Ok(petition);
        }

        [HttpGet("/getListPetition/{page}")]
        public async Task<IActionResult> GetTopics(int page)
        {
            page = page - 1;
            var listPetition = _context.Petitions.Include(a => a.User).Where(a => a.State == Data.Enums.WaitState.Pending && a.Content != "").ToList().Skip(page * 10).Take(10).ToList();

            return Ok(listPetition);
        }

        [HttpGet("/getTotalPagePetition")]
        public async Task<int> GetTotalPage()
        {
            return _context.Petitions.Include(a => a.User).Where(a => a.State == Data.Enums.WaitState.Pending && a.Content != "").Count() / 10;
        }

    }
}
