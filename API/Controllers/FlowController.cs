using BlogWebsite.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlowController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public FlowController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("/GetAllFolwerUser/{IdUser}")]
        public async Task<List<Guid>> GetAllFolwerUser(Guid IdUser)
        {
            var listId = new List<Guid>();
            var result =  _context.Flower.Where(a => a.IdUser == IdUser).Select(a => a.IdFlower).ToList();
            if (result != null)
            {
                listId = result;
            }
            return listId;
        }

        
    }

}
