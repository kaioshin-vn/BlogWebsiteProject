using BlogWebsite.Data;
using Data.Database.Table;
using Data.DTO;
using Data.Enums;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection;
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


		[HttpGet("get-all-memberGroup")]
		public async Task<ActionResult<List<MemberGroup>>> GetAllMember()
		{
			var lstMember = await _context.MemberGroups.ToListAsync();
			return Ok(lstMember);
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
					IdMember = memberId,
					Position = Position.Member,

				};
				_context.MemberGroups.Add(memberGroup);
				await _context.SaveChangesAsync();
				return CreatedAtAction(nameof(CreateMemberGroup), new { groupId, memberId }, memberGroup);
			}
		}

		[HttpGet("get-all-group")]
		public async Task<List<Group>> GetAllGroup()
		{
			var listGroup = _context.Groups.ToListAsync().Result.Take(5);
			return listGroup.ToList();
		}

		//[HttpGet("group-owner/{groupId}")]
		//public async Task<IActionResult> GetGroupOwner(Guid groupId)
		//{
		//	var groupOwner = await _context.MemberGroups
		//		.Include(gm => gm.User) // Bao gồm thông tin người dùng
		//		.Where(gm => gm.IdGroup == groupId && gm.Position == 0) // Lọc để tìm chủ nhóm (position == 0)
		//		.Select(gm => new
		//		{
		//			OwnerId = gm.User.Id,
		//			OwnerName = gm.User.FullName,
		//			OwnerEmail = gm.User.Email
		//		})
		//		.FirstOrDefaultAsync();

		//	if (groupOwner == null)
		//	{
		//		return NotFound("Chủ nhóm không tồn tại.");
		//	}

		//	return Ok(groupOwner);
		//}

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

			var adminGroup = new MemberGroup
			{
				IdGroup = group.IdGroup,
				IdMember = groupDto.UserId,
				Position = Position.Chief,
			};

			_context.MemberGroups.Add(adminGroup);
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
		[HttpDelete("member/{id}")]
		public async Task<ActionResult> DeleteMember(Guid id, [FromQuery] Guid userId)
		{
			var currentUser = userId;
			Console.WriteLine($"abc: {currentUser}");
			Console.WriteLine($"id: {id}");
			if (currentUser == null)
			{
				return Unauthorized("Không tìm thấy người dùng.");
			}

			// Tiếp tục xử lý và xóa thành viên
			var item = await _context.MemberGroups.FirstOrDefaultAsync(a => a.IdMember == id);
			if (item == null)
			{
				return NotFound();  // Trả về 404 nếu không tìm thấy thành viên
			}

			// Kiểm tra quyền của người dùng hiện tại
			var adminGroup = await _context.MemberGroups
				.FirstOrDefaultAsync(ag => ag.IdGroup == item.IdGroup && ag.IdMember == userId);
			if (adminGroup == null || (adminGroup.Position != Position.Chief && adminGroup.Position != Position.Deputy))
			{
				return Unauthorized("Bạn không có quyền xóa thành viên này.");
			}

			// Logic kiểm tra để phó nhóm không thể xóa được phó nhóm hoặc chủ nhóm
			if (adminGroup.Position == Position.Deputy &&
				(item.Position == Position.Deputy || item.Position == Position.Chief))
			{
				return Unauthorized("Chủ nhóm mới có quyền xóa phó nhóm.");
			}

			// Xóa thành viên và phó nhóm nếu có
			_context.MemberGroups.Remove(item);
			//// Xóa phó nhóm nếu người dùng là chủ nhóm
			//if (adminGroup.Position == Position.Chief && item.Position != Position.Deputy)
			//{
			//	var deputy = await _context.MemberGroups
			//		.FirstOrDefaultAsync(ag => ag.IdGroup == item.IdGroup && ag.Position == Position.Deputy);
			//	if (deputy != null)
			//	{
			//		_context.MemberGroups.Remove(deputy);
			//	}
			//}
			await _context.SaveChangesAsync();
			return Ok();
		}


		[HttpPut("update-member/{id}")]
		public async Task<ActionResult> UpdateMember(Guid id, [FromQuery] Guid userId, MemberDTO memberInfo)
		{
			// Lấy thông tin thành viên từ id
			var member = await _context.MemberGroups.FirstOrDefaultAsync(a => a.IdMember == id);
			Console.WriteLine($"Received Id: {id}");
			Console.WriteLine($"MemberInfo: {JsonConvert.SerializeObject(memberInfo.Id)}");

			// Kiểm tra nếu không tìm thấy thành viên
			if (member == null)
			{
				return BadRequest("Member not found.");
			}

			var group = await _context.Groups.FirstOrDefaultAsync(g => g.IdGroup == member.IdGroup);
			if (group == null)
			{
				return BadRequest("Group not found.");
			}

			// Lấy thông tin người dùng hiện tại (người gửi yêu cầu)
			var currentUserId = userId;
			Console.WriteLine($"tan: {currentUserId}");

			// Kiểm tra xem người gửi yêu cầu có phải là chủ nhóm hay không
			var isCurrentUserChief = await _context.MemberGroups
				.AnyAsync(mg => mg.IdGroup == group.IdGroup && mg.IdMember == currentUserId && mg.Position == Position.Chief);

			if (!isCurrentUserChief)
			{
				return Forbid("Only the group owner (chief) can assign deputy roles.");
			}

			// Cấp quyền cho phó nhóm hoặc thay đổi quyền thành viên
			if (memberInfo.Position == Position.Chief)
			{
				// Chuyển thành viên thành chủ nhóm
				var currentChief = await _context.MemberGroups.FirstOrDefaultAsync(mg => mg.IdGroup == group.IdGroup && mg.Position == Position.Chief);
				if (currentChief != null)
				{
					currentChief.Position = Position.Member;
					_context.MemberGroups.Update(currentChief);
				}
				member.Position = Position.Chief;
				_context.MemberGroups.Update(member);
			}
			else if (memberInfo.Position == Position.Deputy)
			{
				// Chuyển thành viên thành phó nhóm
				var adminGroup = new MemberGroup()
				{
					IdGroup = group.IdGroup,
					IdMember = id,
					Position = Position.Deputy,
				};
				_context.MemberGroups.Remove(member); // Xóa quyền cũ
				await _context.MemberGroups.AddAsync(adminGroup); // Thêm quyền mới
			}
			else if (memberInfo.Position == Position.Member)
			{
				// Chuyển phó nhóm hoặc chủ nhóm xuống thành viên thường
				var memberGroup = new MemberGroup()
				{
					IdGroup = group.IdGroup,
					IdMember = id,
					Position = Position.Member,
				};
				_context.MemberGroups.Remove(member); // Xóa quyền cũ
				await _context.MemberGroups.AddAsync(memberGroup); // Thêm quyền mới
			}

			await _context.SaveChangesAsync();
			return Ok("Member position updated successfully.");
		}


	}
}
