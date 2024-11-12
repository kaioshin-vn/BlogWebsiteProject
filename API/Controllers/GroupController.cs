using BlogWebsite.Data;
using Data.Database.Table;
using Data.DTO;
using Data.Enums;
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

		[HttpGet("get-all-group")]
		public async Task<List<Group>> GetAllGroup()
		{
			var listGroup =  _context.Groups.ToListAsync().Result.Take(8);
			return listGroup.ToList();
		}

		[HttpPost("memberGroup")]
		public async Task<IActionResult> CreateMemberGroup([FromBody] MemberGroupRequest request)
		{
			Guid groupId = Guid.Parse(request.GroupId);
			Guid memberId = Guid.Parse(request.MemberId);

			var memberCurrent = await _context.MemberGroups.FirstOrDefaultAsync(a => a.IdGroup == groupId && a.IdMember == memberId);
			if (memberCurrent != null)
			{
				_context.MemberGroups.Remove(memberCurrent);
				await _context.SaveChangesAsync();
				return Ok();
			}
			else
			{
				var memberGroup = new MemberGroup()
				{
					IdGroup = groupId,
					IdMember = memberId
				};
				_context.MemberGroups.Add(memberGroup);
				await _context.SaveChangesAsync();
				return CreatedAtAction(nameof(CreateMemberGroup), new { groupId, memberId }, memberGroup);
			}
		}


		[HttpGet("get-all-memberGroup")]
		public async Task<ActionResult<List<MemberGroup>>> GetAllMember()
		{
			var lstMember = await _context.MemberGroups.ToListAsync();
			return Ok(lstMember);
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
				DateTime = DateTime.Now,
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
				Position = Position.Chief,
			};

			_context.AdminGroups.Add(adminGroup);
			await _context.SaveChangesAsync();


			// Thêm các chủ đề đã chọn vào bảng GroupTopic
			foreach (var topicId in groupDto.Topics)
			{
				var groupTopic = new GroupTopic
				{
					IdGroup = group.IdGroup,
					IdTopic = topicId
				};
				_context.GroupTopics.Add(groupTopic);
			}

			await _context.SaveChangesAsync();


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
