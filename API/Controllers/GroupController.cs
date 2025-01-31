﻿using API.StaticClass;
using BlogWebsite.Data;
using Data.Database.Table;
using Data.DTO;
using Data.DTO.EntitiDTO;
using Data.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;

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

        [HttpGet("/GetListPostPendingGroup/{IdGroup}/{IdUser}")]
        public async Task<List<PostIntroDTO>> GetListPostPendingGroup(Guid IdGroup, Guid IdUser)
        {
            var listPost = await _context.Posts.Include(a => a.User).Include(a => a.GroupPost).ThenInclude(a => a.Group).Where(a => a.GroupPost.Any(p => p.IdGroup == IdGroup && p.WaitState == WaitState.Pending) && a.IdUser == IdUser && a.IsDeleted == false && a.User.LockoutEnd == null ).ToListAsync();

            var listIntroPost = new List<PostIntroDTO>();

            foreach (var item in listPost)
            {
                var intro = await GetPostIntro(item);
                listIntroPost.Add(intro);
            }

            return listIntroPost;
        }

        [HttpGet("get-member/{id}")]
        public async Task<ActionResult<MemberDTO>> GetMemberInfo(Guid id, [FromQuery] string groupName)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("ID không hợp lệ");
            }
            var member = await _context.MemberGroups
                .Where(ag => ag.IdMember == id && ag.Group.Name == groupName)
                .Select(ag => new MemberDTO
                {
                    FullName = ag.User.FullName,
                    Position = ag.Position
                })
                .FirstOrDefaultAsync();
            if (member == null)
            {
                return NotFound("Không tìm thấy thành viên thuộc nhóm này");
            }
            return Ok(member);
        }

        [HttpGet("get-members/{Name}/{Page}/{Search?}")]
        public async Task<ActionResult<List<MemberDTO>>> GetMembersByGroup(string Name, int Page, string? Search)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return BadRequest("Tên nhóm không hợp lệ.");
            }

            Page = Page - 1;

            var members = new List<MemberDTO>();

            if (string.IsNullOrEmpty(Search))
            {
                members = await _context.MemberGroups
                .Include(a => a.User)
                .Include(a => a.Group)
                .Where(ag => ag.Group.Name == Name)
                .Skip(Page * 10).Take(10)
                .Select(ag => new MemberDTO
                {
                    Id = ag.IdMember,
                    FullName = ag.User.FullName,
                    Email = ag.User.Email,
                    Avatar = ag.User.Img,
                    Position = ag.Position
                })
                .ToListAsync();
            }
            else
            {
                members = await _context.MemberGroups
               .Include(a => a.User)
               .Include(a => a.Group)
               .Where(ag => ag.Group.Name == Name && ag.User.FullName.ToLower().Contains(Search.ToLower()))
               .Skip(Page * 10).Take(10)

               .Select(ag => new MemberDTO
               {
                   Id = ag.IdMember,
                   FullName = ag.User.FullName,
                   Email = ag.User.Email,
                   Avatar = ag.User.Img,
                   Position = ag.Position
               })
               .ToListAsync();
            }
            return Ok(members);
        }


        [HttpGet("/gettotalpagemembergroup/{Name}/{Search?}")]
        public async Task<int> GetTotalPageMember(string Name, string? Search)
        {
            if (!string.IsNullOrEmpty(Search))
            {
                return _context.MemberGroups.Include(a => a.User)
                .Include(a => a.Group)
                .Where(ag => ag.Group.Name == Name && ag.User.FullName.ToLower().Contains(Search.ToLower())).Count() / 10;
            }
            else
            {
                return _context.MemberGroups.Include(a => a.User)
                .Include(a => a.Group)
                .Where(ag => ag.Group.Name == Name).Count() / 10;
            }
        }


        [HttpGet("member-count/{groupName}")]
        public async Task<ActionResult<int>> GetMemberCountAsync(string groupName)
        {
            var memberCount = await _context.MemberGroups
                .Where(mg => mg.Group.Name == groupName)
                .CountAsync();

            return Ok(memberCount);
        }

        [HttpGet("state-group/{name}")]
        public async Task<ActionResult<bool>> GetStateGroup(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Tên nhóm không hợp lệ.");
            }

            var state = await _context.Groups
                .AnyAsync(a => a.Name == name && a.StateGroup == KindGroup.Restricted);

            return Ok(state);
        }

        [HttpGet("is-adminOrChief/{groupName}")]
        public async Task<ActionResult<bool>> IsAdminOrChief(string groupName, [FromQuery] Guid userId)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                return BadRequest();
            }
            // Tìm nhóm trong bảng Groups
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Name == groupName);
            if (group == null)
            {
                return NotFound("Không tìm thấy nhóm");
            }
            var currentUser = userId;
            // Lấy IdGroup từ bảng Groups
            var idGroup = group.IdGroup;
            // Kiểm tra xem người dùng có phải là admin của nhóm hay không
            var getAdmin = await _context.MemberGroups
                .Where(a => a.IdGroup == idGroup && a.IdMember == userId
                        && (a.Position == Position.Chief || a.Position == Position.Deputy))
                .FirstOrDefaultAsync();
            return Ok(getAdmin != null);
        }

        [HttpGet("is-admin/{groupName}")]
        public async Task<ActionResult<bool>> IsAdmin(string groupName, [FromQuery] Guid userId)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                return BadRequest();
            }
            // Tìm nhóm trong bảng Groups
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Name == groupName);
            if (group == null)
            {
                return NotFound("Không tìm thấy nhóm");
            }
            var currentUser = userId;
            // Lấy IdGroup từ bảng Groups
            var idGroup = group.IdGroup;
            // Kiểm tra xem người dùng có phải là admin của nhóm hay không
            var getAdmin = await _context.MemberGroups
                .Where(a => a.IdGroup == idGroup && a.IdMember == userId
                        && a.Position == Position.Chief)
                .FirstOrDefaultAsync();
            return Ok(getAdmin != null);
        }

        [HttpDelete("is-chief")]
        public async Task<ActionResult> IsChief([FromQuery] Guid userId, [FromQuery] string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                return BadRequest();
            }
            if (userId == null)
            {
                return Unauthorized("Không tìm thấy người dùng.");
            }

            Console.WriteLine($"Id tantsadsa: {userId}");
            // Check thông tin thành viên bị xóa
            var item = await _context.MemberGroups.Include(mg => mg.Group).FirstOrDefaultAsync(mg => mg.IdMember == userId && mg.Group.Name == groupName);
            var group = item.Group;
            if (group == null)
            {
                return BadRequest("Không tìm thấy nhóm của thành viên.");
            }

            Console.WriteLine($"Id nhóm: {group.IdGroup}, Tên nhóm: {group.Name}");
            if (item == null)
            {
                return NotFound("Không tìm thấy thành viên.");
            }
            _context.MemberGroups.Remove(item);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("is-member/{groupName}")]
        public async Task<ActionResult<bool>> IsMember(string groupName, [FromQuery] Guid userId)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                return BadRequest();
            }
            // Tìm nhóm trong bảng Groups
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Name == groupName);
            if (group == null)
            {
                return NotFound("Không tìm thấy nhóm");
            }
            var currentUser = userId;
            // Lấy IdGroup từ bảng Groups
            var idGroup = group.IdGroup;
            // Kiểm tra xem người dùng có phải là thành viên của nhóm hay không
            var getMember = await _context.MemberGroups
                .Where(a => a.IdGroup == idGroup && a.IdMember == userId
                        && a.Position == Position.Member)
                .FirstOrDefaultAsync();
            return Ok(getMember != null);
        }

        [HttpGet("is-member-creatPost/{groupId}")]
        public async Task<ActionResult<bool>> IsMemberCreatePost(Guid groupId, [FromQuery] Guid userId)
        {

            // Tìm nhóm trong bảng Groups
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.IdGroup == groupId);
            if (group == null)
            {
                return NotFound("Không tìm thấy nhóm");
            }
            if (group.StateGroup != KindGroup.Restricted)
            {
                return BadRequest("Chỉ áp dụng cho nhóm có trạng thái Private");
            }
            var currentUser = userId;
            // Lấy IdGroup từ bảng Groups
            var idGroup = group.IdGroup;
            // Kiểm tra xem người dùng có phải là thành viên của nhóm hay không
            var getMember = await _context.MemberGroups
                .Where(a => a.IdGroup == idGroup && a.IdMember == userId
                        && a.Position == Position.Member)
                .FirstOrDefaultAsync();
            return Ok(getMember != null);
        }

        [HttpPost("memberGroup")]
        public async Task<IActionResult> CreateMemberGroup([FromBody] MemberGroupRequest request)
        {
            Guid groupId = Guid.Parse(request.GroupId);
            Guid memberId = Guid.Parse(request.MemberId);

            var memberCurrent = await _context.MemberGroups.FirstOrDefaultAsync(a => a.IdGroup == groupId && a.IdMember == memberId);
            if (memberCurrent != null)
            {
                var listPostGroup = _context.GroupPosts.Include(a => a.Post).Include(a => a.Group).ThenInclude(a => a.MemberGroups).Where(a => a.IdGroup == groupId).ToList();

                if (listPostGroup != null && listPostGroup.Count != 0)
                {
                    var listPost = new List<Post>();

                    var listPostMember = listPostGroup.Where(a => a.Post.IdUser == memberId).ToList();

                    if (listPostMember.Count != 0 && listPostMember != null)
                    {
                        listPost = listPostMember.Select(a => a.Post).ToList();
                        foreach (var item in listPost)
                        {
                            item.IsDeleted = true;
                        }
                        _context.Posts.UpdateRange(listPost);
                    }
                }

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

        [HttpDelete("/deleteGroup/{IdGr}")]
        public async Task DeleteGroup(Guid IdGr)
        {
            var gr = await _context.Groups.FirstOrDefaultAsync(a => a.IdGroup == IdGr);
            gr.isDeleted = true;
            _context.Groups.Update(gr);

            var listPost = _context.GroupPosts.Include(a => a.Post).Where(a => a.IdGroup == IdGr).ToList();
            foreach (var item in listPost)
            {
                var post = item.Post;
                post.IsDeleted = true;
                _context.Posts.Update(post);
            }

            await _context.SaveChangesAsync();
        }

        [HttpGet("get-all-groups")]
        public async Task<List<Group>> GetAllGroups()
        {
            var listGroup = await _context.Groups.ToListAsync();
            return listGroup;
        }

        [HttpGet("get-all-memberCount-group")]
        public async Task<List<GroupDTO>> GetAllGroupsWithMemberCount()
        {
            var groups = await _context.Groups
                .GroupJoin(
                    _context.MemberGroups,
                    group => group.IdGroup,
                    userGroup => userGroup.IdGroup,
                    (group, userGroups) => new { group, userGroups }
                )
                .Select(g => new GroupDTO
                {
                    IdGroup = g.group.IdGroup,
                    Name = g.group.Name,
                    Description = g.group.Description,
                    ImgGroup = g.group.ImgGroup,
                    ImgCover = g.group.ImgCover,
                    StateGroup = g.group.StateGroup,
                    Topics = g.group.GroupTopics.Select(t => t.IdTopic).ToList(),

                    // Lấy UserId của người tạo nhóm
                    UserId = g.userGroups
                        .Where(ug => ug.Position == 0) // Điều kiện xác định người tạo nhóm
                        .Select(ug => ug.IdMember)
                        .FirstOrDefault(), // Lấy UserId đầu tiên nếu có

                    // Đếm số lượng thành viên
                    MemberCount = g.userGroups.Count()
                })
                .ToListAsync();

            return groups;
        }

        [HttpGet("get-all-group-user")]
        public async Task<List<Group>> GetAllGroup([FromQuery] Guid userId)
        {
            var listGroup = await _context.MemberGroups.Include(a => a.Group)
                                          .Where(mg => mg.IdMember == userId && mg.Group.isDeleted == false)
                                          .Select(mg => mg.Group)
                                          .ToListAsync();
            return listGroup;
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
            bool nameExists = await _context.Groups.AnyAsync(g => g.Name == name && g.isDeleted == false);
            return Ok(nameExists);
        }

        [HttpGet("/isgroupprivate/{Id}")]
        public async Task<bool> IsGroupPrivate(Guid Id)
        {
            var gr = await _context.Groups.FirstOrDefaultAsync(a => a.IdGroup == Id);
            return gr.StateGroup == KindGroup.Private;
        }

        [HttpDelete("member/{id}")]
        public async Task<ActionResult> DeleteMember(Guid id, [FromQuery] Guid userId, [FromQuery] string groupName)
        {
            Console.WriteLine($"abc: {userId}");
            Console.WriteLine($"id: {id}");
            if (userId == null)
            {
                return Unauthorized("Không tìm thấy người dùng.");
            }

            // Check thông tin thành viên bị xóa
            var item = await _context.MemberGroups.Include(mg => mg.Group).FirstOrDefaultAsync(mg => mg.IdMember == id && mg.Group.Name == groupName);

            if (item == null)
            {
                return NotFound("Không tìm thấy thành viên.");
            }
            var group = item.Group;
            if (group == null)
            {
                return BadRequest("Không tìm thấy nhóm của thành viên.");
            }

            Console.WriteLine($"Id nhóm: {group.IdGroup}, Tên nhóm: {group.Name}");

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



            var listPostGroup = _context.GroupPosts.Include(a => a.Post).Include(a => a.Group).ThenInclude(a => a.MemberGroups).Where(a => a.IdGroup == group.IdGroup).ToList();

            if (listPostGroup != null && listPostGroup.Count != 0)
            {
                var listPost = new List<Post>();

                var listPostMember = listPostGroup.Where(a => a.Post.IdUser == id).ToList();

                if (listPostMember.Count != 0 && listPostMember != null)
                {
                    listPost = listPostMember.Select(a => a.Post).ToList();
                    foreach (var it in listPost)
                    {
                        it.IsDeleted = true;
                    }
                    _context.Posts.UpdateRange(listPost);
                }
            }

            _context.MemberGroups.Remove(item);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("/GroupUserJoin/{idUser}")]
        public async Task<List<GroupInfoDTO>> GetListGroupUserJoin(Guid idUser)
        {
            var listIdGroup = _context.MemberGroups.Where(a => a.IdMember == idUser).Select(a => a.IdGroup).ToList();
            var listGr = new List<GroupInfoDTO>();

            listGr = _context.Groups.Where(a => listIdGroup.Contains(a.IdGroup)).Select(a =>
                 new GroupInfoDTO()
                 {
                     IdGroup = a.IdGroup,
                     ImgGroup = a.ImgGroup,
                     Name = a.Name
                 }).ToList();

            return listGr;
        }

        [HttpPost("/UserUpPostOnGroup/{IdGroup}")]
        public async Task<IActionResult> UserUpPostOnGroup(Guid IdGroup, [FromBody] Guid IdPost, [FromQuery] Guid userId)
        {
            var groupPost = new GroupPost
            {
                IdGroup = IdGroup,
                IdPost = IdPost
            };

            // Kiểm tra nếu userId là trưởng nhóm hoặc phó nhóm
            var getAdmin = await _context.MemberGroups
                .Where(a => a.IdGroup == IdGroup && a.IdMember == userId
                        && (a.Position == Position.Chief || a.Position == Position.Deputy))
                .FirstOrDefaultAsync();

            var group = await _context.Groups.FirstOrDefaultAsync(a => a.IdGroup == IdGroup);

            if (getAdmin != null)
            {
                groupPost.WaitState = WaitState.Accept;
            }
            else
            {
                if (group.StateGroup == KindGroup.Public)
                {
                    groupPost.WaitState = WaitState.Accept;
                }
                else if (group.StateGroup == KindGroup.Restricted)
                {
                    groupPost.WaitState = WaitState.Pending;
                }
                else if (group.StateGroup == KindGroup.Private)
                {
                    groupPost.WaitState = WaitState.Accept;
                }
            }

            _context.GroupPosts.Add(groupPost);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("update-group-info")]
        public async Task<ActionResult> UpdateGroupInfo([FromQuery] string groupName, [FromBody] Group group)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                return BadRequest("Tên nhóm không được để trống!");
            }

            var finGroup = await _context.Groups.FirstOrDefaultAsync(a => a.Name == groupName);
            if (finGroup == null)
            {
                return BadRequest("Không tìm thấy nhóm!!!");
            }
            finGroup.Name = group.Name;
            finGroup.Description = group.Description;
            finGroup.ImgCover = group.ImgCover;
            finGroup.ImgGroup = group.ImgGroup;
            finGroup.StateGroup = group.StateGroup;
            _context.Groups.Update(finGroup);
            await _context.SaveChangesAsync();
            return Ok();

        }

        [HttpPut("update-member/{id}")]
        public async Task<ActionResult> UpdateMember(Guid id, [FromQuery] Guid userId, [FromQuery] string groupName, [FromBody] MemberDTO memberInfo)
        {
            Console.WriteLine($"anhbuon:{groupName}");
            // Lấy thông tin thành viên từ id
            var member = await _context.MemberGroups.Include(a => a.Group).FirstOrDefaultAsync(a => a.IdMember == id && a.Group.Name == groupName);
            Console.WriteLine($"MemberInfo: {JsonConvert.SerializeObject(memberInfo.Id)}");

            // Kiểm tra nếu không tìm thấy thành viên
            if (member == null)
            {
                return BadRequest("Không tìm thấy thành viên.");
            }
            // Lấy thông tin người dùng hiện tại (người gửi yêu cầu)
            var currentUserId = userId;
            Console.WriteLine($"tan: {currentUserId}");

            // Kiểm tra xem người gửi yêu cầu có phải là chủ nhóm hay không
            var isCurrentUserChief = await _context.MemberGroups.Include(a => a.Group)
                .AnyAsync(mg => mg.Group.Name == groupName && mg.IdMember == userId && mg.Position == Position.Chief);
            Console.WriteLine($"tantttttt: {isCurrentUserChief}");

            if (!isCurrentUserChief)
            {
                return Forbid("Chỉ có nhóm trưởng mới có thể chỉ định vai trò.");
            }
            // Cấp quyền cho phó nhóm hoặc thay đổi quyền thành viên
            if (memberInfo.Position == Position.Chief)
            {
                // Chuyển thành viên thành chủ nhóm
                var currentChief = await _context.MemberGroups.Include(a => a.Group).FirstOrDefaultAsync(mg => mg.Group.Name == groupName && mg.Position == Position.Chief);
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
                // Cập nhật thành Deputy
                member.Position = Position.Deputy;
                _context.MemberGroups.Update(member);
            }
            else if (memberInfo.Position == Position.Member)
            {  // Hạ cấp xuống Member
                member.Position = Position.Member;
                _context.MemberGroups.Update(member);
            }

            await _context.SaveChangesAsync();
            return Ok("Cập nhật thành công");
        }
        private async Task<PostIntroDTO> GetPostIntro(Post post)
        {
            string Avatar;
            string UserName;

            var introPost = post.GetIntroPost();
            var groupPost = _context.GroupPosts.FirstOrDefault(a => a.IdPost == post.Id);



            var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == post.IdUser);
            UserName = user.FullName;
            Avatar = user.Img;
            var link = "/other-profile/" + user.Id.ToString();

            var commment = _context.Responses.Where(a => a.IdPost == post.Id && a.IsDeleted == false);
            var replyCount = 0;
            var replyComment = _context.ReplyResponses.Where(a => commment.Any(b => b.Id == a.IdResponse && a.IsDeleted == false));

            var tagNames = new List<string>();
            var listTag = _context.PostTags
                            .Where(a => a.IdPost == post.Id)
                            .Select(a => a.IdTag)
                            .ToList();
            if (listTag != null && listTag.Count > 0)
            {
                tagNames = _context.Tags
                                    .Where(tag => listTag.Contains(tag.Id))
                                    .Select(tag => tag.TagName)
                                    .ToList();
            }


            var totalReply = commment.Count() + replyComment.Count();

            introPost.CommentCount = totalReply;
            introPost.UserName = UserName;
            introPost.Avatar = Avatar;
            introPost.ListTag = tagNames;
            introPost.Link = link;
            return introPost;
        }
    }
}
