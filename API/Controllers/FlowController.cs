using BlogWebsite.Data;
using Data.DTO.EntitiDTO;
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

        [HttpGet("/GetAllUserFlow/{IdUser}/{Search?}")]
        public async Task<List<UserDto>> GetAllUserFolow(Guid IdUser, string? Search)
        {
            var listUser = new List<UserDto>();
            var result = _context.Flower.Where(a => a.IdFlower == IdUser && a.IsFollowing == true).Select(a => a.IdUser).ToList();
            if (result != null)
            {
                if (string.IsNullOrEmpty(Search))
                {
                    listUser = _context.Users.Where(a => result.Contains(a.Id)).Select(a => new UserDto
                    {
                        Id = a.Id.ToString(),
                        FullName = a.FullName,
                        Img = a.Img,
                        IsFollowing = true,

                    }).ToList();
                }
                else
                {
                    listUser = _context.Users.Where(a => result.Contains(a.Id) && (a.FullName.ToLower().Contains(Search.ToLower()) || a.UserName.ToLower().Contains(Search.ToLower()))).Select(a => new UserDto
                    {
                        Id = a.Id.ToString(),
                        FullName = a.FullName,
                        Img = a.Img,
                        IsFollowing = true,

                    }).ToList();
                }
            }

            return listUser;

        }

    }

}
