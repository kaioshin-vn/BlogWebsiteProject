using API.DTO;
using BlogWebsite.Data;
using Data.Database.Table;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace API.Controllers.UserController
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }
            return new UserDto
            {
                Id = id,
                Descript = user.Descript,
                Img = user.Img,
                ImgCover = user.ImgCover,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                FullName = user.FullName,
                CountFollow = _context.Flower.Where(c => c.IdUser == Guid.Parse(id) && c.IsFollowing == true).Count(),
            };
        }
        [HttpGet("{id}/{idViewer}")]
        public async Task<ActionResult<UserDto>> GetUserViewr(Guid id, Guid idViewer)
        {
            var user = await _userManager.FindByIdAsync(idViewer.ToString());
            if (user == null)
            {
                return null;
            }
            var isFl = _context.Flower.FirstOrDefault(c => c.IdFlower == id && c.IdUser == idViewer);
            return new UserDto
            {
                Id = id.ToString(),
                IdViewer = idViewer.ToString(),
                Descript = user.Descript,
                Img = user.Img,
                ImgCover = user.ImgCover,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                FullName = user.FullName,
                CountFollow = _context.Flower.Where(c => c.IdUser == idViewer && c.IsFollowing == true).Count(),
                IsFollowing = isFl == null ? false : isFl.IsFollowing,
            };
        }

        [HttpPost("{id}/{idViewer}/{isFl}/toggle-follow")]
        public async Task<IActionResult> ToggleFollowUser(Guid id /*th follow*/, Guid? idViewer/*th bấm follow*/, bool isFl)
        {
            if (idViewer == null)
            {
                return Unauthorized();
            }
            var fl = await _context.Flower.FirstOrDefaultAsync(c => c.IdUser == id && c.IdFlower == idViewer.Value);
            if (fl != null)
            {
                fl.IsFollowing = !isFl;
                _context.Update(fl);
                await _context.SaveChangesAsync();
            }
            else
            {
                var fln = new Flower()
                {
                    IdFlower = idViewer.Value,
                    IdUser = id,
                    IsFollowing = true
                };
                await _context.Flower.AddAsync(fln);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }
        [HttpPut("{id}")]
        public async Task UpdateUser(string id, [FromBody]UserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Email = userDto.Email;
                user.Address = userDto.Address;
                user.PhoneNumber = userDto.PhoneNumber;
                user.Descript = userDto.Descript;
                user.FullName = userDto.FullName;
                user.Img = userDto.Img;
                await _userManager.UpdateAsync(user);
            }
        }
    }
}
