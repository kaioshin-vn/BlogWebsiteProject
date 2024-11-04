using BlogWebsite.Data;
using Data.Database.Table;
using Data.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GroupController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		public GroupController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpPost("groupDto")]
		public async Task<IActionResult> CreateGroup([FromBody] GroupDTO groupDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var group = new Group()
			{
				IdGroup = Guid.NewGuid(),
				Name = groupDto.Name,
				Description = groupDto.Description,
				ImgCover = groupDto.ImgCover,
				ImgGroup = groupDto.ImgGroup,
				StateGroup = groupDto.StateGroup,
			};

			_context.Groups.Add(group);
			await _context.SaveChangesAsync();

			// Kiểm tra người dùng tồn tại trước khi thêm AdminGroup
			var checkUser = await _context.Users.AnyAsync(u => u.Id == groupDto.UserId);
			if (!checkUser)
			{
				return BadRequest("Người dùng không tồn tại.");
			}

			var adminGroup = new AdminGroup
			{
				IdGroup = group.IdGroup,
				IdAdmin = groupDto.UserId,
			};

			try
			{
				_context.AdminGroups.Add(adminGroup);
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return StatusCode(500, "Lỗi khi thêm admin group");
			}

			return CreatedAtAction(nameof(GetGroupById), new { id = group.IdGroup }, group);
		}



		[HttpGet("groupDto/{id}")]
		public async Task<ActionResult<Group>> GetGroupById(Guid id)
		{
			var group = await _context.Groups.FindAsync(id);
			if (group == null)
			{
				return NotFound();
			}
			return group;
		}

		[HttpGet("check")]
		public async Task<IActionResult> CheckGroupName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return BadRequest("Tên nhóm không được để trống.");
			}


			bool nameExists = await _context.Groups.AnyAsync(g => g.Name == name);
			return Ok(nameExists);
		}

	}
}
