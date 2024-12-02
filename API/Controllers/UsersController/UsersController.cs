using BlogWebsite.Data;
using Client.Components.Pages;
using Data.Database.Table;
using Data.DTO.EntitiDTO;
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
        public async Task<ActionResult<UserProfileDto>> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }
            return new UserProfileDto
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

        [HttpGet("/api/getUserById/{IdUser}")]
        public async Task<ActionResult<UserProfileDto>> GetUserById(Guid IdUser)
        {
            var user = await _context.Users.FirstOrDefaultAsync(a => a.Id ==  IdUser);
            if (user == null)
            {
                return null;
            }
            return new UserProfileDto
            {
                Id = user.Id.ToString(),
                Descript = user.Descript,
                Img = user.Img,
                ImgCover = user.ImgCover,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                FullName = user.FullName,
                CountFollow = _context.Flower.Where(c => c.IdUser == IdUser && c.IsFollowing == true).Count(),
            };
        }

        [HttpGet("{id}/{idViewer}")]
        public async Task<ActionResult<UserProfileDto>> GetUserViewr(Guid id, Guid idViewer)
        {
            var user = await _userManager.FindByIdAsync(idViewer.ToString());
            if (user == null)
            {
                return null;
            }
            var isFl = _context.Flower.FirstOrDefault(c => c.IdFlower == id && c.IdUser == idViewer);
            return new UserProfileDto
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

        [HttpPut("{id}")]
        public async Task UpdateUser(string id, [FromBody] UserProfileDto userDto)
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
        [HttpGet("followers/{userId}")]
        public async Task<FollowerDto> GetFollowersAsync(string userId, [FromQuery] string? searchTerm, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var idGuid))
            {

                var query = _context.Flower
                    .Where(f => f.IdUser == idGuid)
                    .Select(f => f.UserFlower);
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(u => u.FullName.ToLower().Contains(searchTerm.ToLower().Trim()));
                }
                var totalCount = await query.CountAsync();

                var users = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                var followers = new List<UserFollower>();
                foreach (var u in users)
                {
                    var isFollowing = await IsFollowingAsync(u.Id, idGuid);
                    followers.Add(new UserFollower
                    {
                        Id = u.Id,
                        FullName = u.FullName,
                        Img = u.Img,
                        IsFollowing = isFollowing
                    });
                }
                return new FollowerDto
                {
                    Followers = followers,
                    TotalCount = totalCount
                };
            }
            else
            {
                return new FollowerDto
                {
                    Followers = new List<UserFollower>(),
                    TotalCount = 0
                };
            }
        }
        private async Task<bool> IsFollowingAsync(Guid followerId, Guid IdUser)
        {
            // chỗ này phải ngược lại vì mìn hđang kiểm tra xem mình có follow những người follow mình không
            return await _context.Flower
                .AnyAsync(f => f.IdFlower == IdUser && f.IdUser == followerId && f.IsFollowing);
        }

        [HttpGet("getListUser")]
        public async Task<ActionResult<List<UserDto>>> GetListUser()
        {
            var lstUser = await _userManager.Users.ToListAsync();
            var userDtos = lstUser.Select(user => new UserDto
            {
                Id = user.Id.ToString(),
                Descript = user.Descript,
                Img = user.Img,
                ImgCover = user.ImgCover,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                FullName = user.FullName,
            }).ToList();
            return Ok(userDtos);
        }

    }
    public class FollowerDto
    {
        public List<UserFollower> Followers { get; set; }
        public int TotalCount { get; set; }
    }
    public class UserFollower
    {
        public string FullName { get; set; }
        public string Img { get; set; }
        public Guid Id { get; set; }
        public bool IsFollowing { get; set; }
    }
}
