using API.StaticClass;
using BlogWebsite.Data;
using Data.Database;
using Data.Database.Table;
using Data.DTO;
using Data.DTO.EntitiDTO;
using Data.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Claims;

namespace API.Controllers.PostController
{
	[Route("api/[controller]")]
	[ApiController]
	public class PostController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment _environment;
		public PostController(ApplicationDbContext context, IWebHostEnvironment environment)
		{
			_context = context;
			_environment = environment;
		}

		[HttpGet("getPostPagination")]
		public IActionResult GetAllPostPage(int page = 1, string inputSearch = null)
		{
			int pageSize = 10;
			// Truy vấn csdl
			var postQuery = _context.Posts.Where(p => !p.IsDeleted).ToList(); // lst ko chứa post đã xoá

			// Tìm kiếm theo tiêu đề
			if (!string.IsNullOrEmpty(inputSearch))
			{
				postQuery = postQuery.Where(x => x.Title.Contains(inputSearch)).ToList();
			}
			// Tính tổng số trong
			var totalPages = (int)Math.Ceiling((decimal)postQuery.Count / pageSize);
			// Lấy danh sách phân trang
			var posts = postQuery
						.Skip((page - 1) * pageSize) // Bỏ qua số bài viết trên các trang trước
						.Take(pageSize) // Lấy số bài viết cho trang hiện tại
						.ToList();

			return Ok(new Pagination<Post>()
			{
				TotalPage = totalPages,
				ListPage = posts
			});
		}

		[HttpPost("/GetListPostIntro/{IdUser?}")]
		public async Task<List<PostIntroDTO>> GetListPostIntro(Guid? IdUser, [FromBody] List<Guid> listPostExisted)
		{
			var listIdHide = _context.PostHideByRestricted.Select(a => a.IdPost).ToList();
			if (IdUser != null)
			{
				var listHide = _context.PostHides.Where(a => a.IdUser == IdUser).Select(a => a.IdPost).ToList();
				if (listHide.Count != 0)
				{
					listIdHide.AddRange(listHide);
				}
			}


			var listPost = await _context.Posts.Include(a => a.User).Include(a => a.GroupPost).ThenInclude(a => a.Group).Where(a => a.IsDeleted == false &&  (a.GroupPost.Count == 0 || 
			(a.GroupPost.Any(b => b.IdPost == a.Id && b.WaitState == WaitState.Accept  && ( b.Group.StateGroup == KindGroup.Public ))))
			&& !listPostExisted.Contains(a.Id) && a.User.LockoutEnd == null && !listIdHide.Contains(a.Id))
				.OrderByDescending(a => a.CreateDate).Take(20).ToListAsync();

            var listIntroPost = new List<PostIntroDTO>();
			foreach (var item in listPost)
			{
				var introPost = await GetPostIntro(item);
				listIntroPost.Add(introPost);
			}
			return listIntroPost;
		}

        [HttpPost("/GetListPostHot/{IdUser?}")]
        public async Task<List<PostIntroDTO>> GetListPostIntroHot(Guid? IdUser, [FromBody] List<Guid> listPostExisted)
        {
            var listIdHide = _context.PostHideByRestricted.Select(a => a.IdPost).ToList();
            if (IdUser != null)
            {
                var listHide = _context.PostHides.Where(a => a.IdUser == IdUser).Select(a => a.IdPost).ToList();
                if (listHide.Count != 0)
                {
                    listIdHide.AddRange(listHide);
                }
            }

			var listPost = await _context.Posts.Include(a => a.User).Include(a => a.GroupPost).ThenInclude(a => a.Group).Where(a => a.IsDeleted == false && (a.GroupPost.Count == 0 ||
			(a.GroupPost.Any(b => b.IdPost == a.Id && b.WaitState == WaitState.Accept && (b.Group.StateGroup == KindGroup.Public))))
			&& !listPostExisted.Contains(a.Id) && a.User.LockoutEnd == null && !listIdHide.Contains(a.Id) && a.CreateDate >= DateTime.Now.AddDays(-7))
				.OrderByDescending(a => a.Like.Length ).Take(20).ToListAsync();

            var listIntroPost = new List<PostIntroDTO>();
            foreach (var item in listPost)
            {
                var introPost = await GetPostIntro(item);
                listIntroPost.Add(introPost);
            }
            return listIntroPost;
        }


        [HttpGet("/GetListPostUser/{idUser}")]
        public async Task<List<PostIntroDTO>> GetListPostUser(Guid idUser)
        {
            var listIdHide = _context.PostHideByRestricted.Select(a => a.IdPost).ToList();
            if (idUser != Guid.Empty)
            {
                var listHide = _context.PostHides.Where(a => a.IdUser == idUser).Select(a => a.IdPost).ToList();
                if (listHide.Count != 0)
                {
                    listIdHide.AddRange(listHide);
                }
            }
            var listPost = await _context.Posts.Include(a => a.GroupPost).Where(a => a.IdUser == idUser && a.GroupPost.Count == 0 && a.IsDeleted == false && !listIdHide.Contains(a.Id)).OrderByDescending(a => a.CreateDate).ToListAsync();

            var listIntroPost = new List<PostIntroDTO>();
            foreach (var item in listPost)
            {
                var introPost = await GetPostIntro(item);
                listIntroPost.Add(introPost);
            }
            return listIntroPost;
        }


        [HttpGet("getListPostWaiState")]
		public async Task<ActionResult<List<PostIntroDTO>>> GetListPostWaiState([FromQuery] string groupName, [FromQuery] Guid? userId)
		{
			var group = await _context.Groups.FirstOrDefaultAsync(a => a.Name == groupName);

			if (group == null)
			{
				return new List<PostIntroDTO>();
			}

			var listPost = await _context.Posts.Where(a => a.IsDeleted == false).ToListAsync();

			// Lấy danh sách GroupPosts cho nhóm này để kiểm tra quan hệ
			var groupPosts = await _context.GroupPosts
											.Include(a => a.Post)
											.ThenInclude(a => a.User)
											.Where(gp => gp.IdGroup == group.IdGroup && gp.WaitState == WaitState.Pending && gp.Post.IsDeleted == false && gp.Post.User.LockoutEnd == null)
											.ToListAsync();

			var listIntroPost = new List<PostIntroDTO>();

			foreach (var item in listPost)
			{
				// Kiểm tra xem bài viết có thuộc nhóm hiện tại không
				var groupPost = groupPosts.FirstOrDefault(gp => gp.IdPost == item.Id && gp.IdGroup == group.IdGroup);
				if (groupPost != null)
				{
					var author = await _context.Users.Include(a => a.Post).FirstOrDefaultAsync(u => u.Id == item.IdUser);
					var introPost = await GetPostIntro(item);
					introPost.UserName = author.UserName;
					introPost.Avatar = author.Img;
					listIntroPost.Add(introPost);
				}
			}
			return listIntroPost;
		}

		[HttpGet("getListPostIntroGroup/{IdUser?}")]
		public async Task<ActionResult<List<PostIntroDTO>>> GetListPostIntroGroup(Guid? IdUser, [FromQuery] string groupName)
		{

			var group = await _context.Groups.FirstOrDefaultAsync(a => a.Name == groupName);

			if (group == null)
			{
				return new List<PostIntroDTO>();
			}


            var listIdHide = _context.PostHideByRestricted.Select(a => a.IdPost).ToList();
            if (IdUser != null)
            {
                var listHide = _context.PostHides.Where(a => a.IdUser == IdUser).Select(a => a.IdPost).ToList();
                if (listHide.Count != 0)
                {
                    listIdHide.AddRange(listHide);
                }
            }


            var listPost = await _context.Posts.Include(a => a.User).Where(a => a.IsDeleted == false && !listIdHide.Contains(a.Id) && a.User.LockoutEnd == null).OrderByDescending(a => a.CreateDate).ToListAsync();

			var groupPosts = new List<GroupPost>();
            // Lấy danh sách GroupPosts cho nhóm này để kiểm tra quan hệ
            if (group.StateGroup != KindGroup.Private)
			{
                groupPosts = await _context.GroupPosts
                                            .Where(gp => gp.IdGroup == group.IdGroup && gp.WaitState == WaitState.Accept)
                                            .ToListAsync();
            }
			else
			{
				var listIdAdmin = _context.MemberGroups.Where(a => a.IdGroup == group.IdGroup && a.Position == Position.Deputy || a.Position == Position.Chief).Select(a => a.IdMember);

                groupPosts = await _context.GroupPosts
					.Include(a => a.Post)
					.ThenInclude(a => a.User)
                                            .Where(gp => gp.IdGroup == group.IdGroup && gp.WaitState == WaitState.Accept && listIdAdmin.Contains(gp.Post.User.Id))
                                            .ToListAsync();   

            }

			var listIntroPost = new List<PostIntroDTO>();

			foreach (var item in listPost)
			{
				// Kiểm tra xem bài viết có thuộc nhóm hiện tại không
				var groupPost = groupPosts.FirstOrDefault(gp => gp.IdPost == item.Id && gp.IdGroup == group.IdGroup);
				if (groupPost != null)
				{
					var author = await _context.Users.Include(a => a.Post).FirstOrDefaultAsync(u => u.Id == item.IdUser);
					var introPost = await GetPostIntro(item);
					introPost.UserName = author.FullName;
					introPost.Avatar = author.Img;
					listIntroPost.Add(introPost);
				}
			}
			return listIntroPost;
		}
		[HttpGet("getListPostIntroGroupForUser")]
		public async Task<ActionResult<List<PostIntroDTO>>> GetListPostIntroGroupForUser([FromQuery] string groupName, [FromQuery] Guid? userId)
		{
			Console.WriteLine($"idUser: {userId}");

			var group = await _context.Groups.FirstOrDefaultAsync(a => a.Name == groupName);

			if (group == null)
			{
				return new List<PostIntroDTO>();
			}

			var groupPosts = await _context.GroupPosts
											.Where(gp => gp.IdGroup == group.IdGroup && gp.WaitState == WaitState.Accept).Select(a => a.IdPost)
											.ToListAsync();
			var listPost = await _context.Posts.Include(a => a.User).Where(a => a.IsDeleted == false && a.IdUser == userId && groupPosts.Contains(a.Id) && a.User.LockoutEnd == null).ToListAsync();

			// Lấy danh sách GroupPosts cho nhóm này để kiểm tra quan hệ

			var listIntroPost = new List<PostIntroDTO>();

			foreach (var item in listPost)
			{
				// Kiểm tra xem bài viết có thuộc nhóm hiện tại không

				var author = await _context.Users.Include(a => a.Post).FirstOrDefaultAsync(u => u.Id == item.IdUser);
				var introPost = await GetPostIntro(item);
				introPost.UserName = author.UserName;
				introPost.Avatar = author.Img;
				listIntroPost.Add(introPost);
			}
			return listIntroPost;
		}


		[HttpPut("/updatePostWaistate/{idPost}")]
		public async Task<IActionResult> UpdatePostWaistate(Guid idPost)
		{
			var findGroupPost = await _context.GroupPosts.FirstOrDefaultAsync(a => a.IdPost == idPost);
			if (findGroupPost == null)
			{
				return BadRequest("Không tìm thấy bài viết trong nhóm");
			}
			findGroupPost.WaitState = WaitState.Accept;
			_context.GroupPosts.Update(findGroupPost);
			await _context.SaveChangesAsync();
			return Ok();
		}

		[HttpPut("/updatePostWaistate2/{idPost}")]
		public async Task<IActionResult> UpdatePostWaistate2(Guid idPost)
		{
			var findGroupPost = await _context.GroupPosts.FirstOrDefaultAsync(a => a.IdPost == idPost);
			if (findGroupPost == null)
			{
				return BadRequest("Không tìm thấy bài viết trong nhóm");
			}
			findGroupPost.WaitState = WaitState.Decline;
			_context.GroupPosts.Update(findGroupPost);
			await _context.SaveChangesAsync();
			return Ok();
		}

		[HttpPost("/createPost")]
		public async Task<IActionResult> CreatePostAsync([FromBody] PostDTO _post)
		{
			if (_post.IdUser == null)
			{
				return BadRequest();
			}
			var post = new Post()
			{
				Id = _post.Id,
				IdUser = _post.IdUser,
				Title = _post.Title,
				Content = _post.Content,
				CreateDate = DateTime.Now,
				VideoFile = _post.VideoFile,
				ImgFile = _post.ImgFile,
				Like = ""
			};
			_context.Posts.Add(post);
			await _context.SaveChangesAsync();
			return Ok(post);
		}

		[HttpGet("/getLikePost/{idPost}")]
		public async Task<string> GetLikePostAsync(Guid idPost)
		{
			var post = _context.Posts.FirstOrDefault(a => a.Id == idPost);
			if (post != null)
			{
				return post.Like;
			}
			else
			{
				return "";
			}
		}

		[HttpGet("/likePost/{idPost}/{idUser}/{likeState}")]
		public async Task<IActionResult> LikePost(Guid idPost, Guid idUser, bool likeState)
		{
			var existingPost = await _context.Posts.FirstOrDefaultAsync(p => p.Id == idPost);
			if (existingPost != null)
			{
				List<string> listLike;
				if (existingPost.Like == null || existingPost.Like == "")
				{
					listLike = new List<string>();
				}
				else
				{
					listLike = existingPost.Like.Split("|").ToList();
				}
				if (likeState)
				{
					if (listLike.Count == 0)
					{
						listLike.Add(idUser.ToString());
					}
					else if (listLike.Any(a => a != idUser.ToString()))
					{
						listLike.Add(idUser.ToString());
					}
				}
				else
				{
					listLike.Remove(idUser.ToString());
				}
				existingPost.Like = string.Join('|', listLike);

				_context.Posts.Update(existingPost);
				await _context.SaveChangesAsync();
				return Ok(existingPost.Like);
			}
			else
			{
				return NotFound();
			}
		}

		[HttpPut("/updatePost/{idPost}")]
		public async Task<IActionResult> EditPost(Guid idPost, [FromBody] PostDTO post)
		{

			var existingPost = await _context.Posts.FirstOrDefaultAsync(p => p.Id == idPost);
			if (existingPost == null)
			{
				return NotFound(); // không tìm thầy bài viết
			}
			existingPost.Title = post.Title;
			existingPost.Content = post.Content;
			if (post.VideoFile != null && post.VideoFile != null)
			{
				existingPost.VideoFile = post.VideoFile;
			}
			existingPost.EditDate = DateTime.Now;
			if (post.ImgFile != null && post.ImgFile != null)
			{
				existingPost.ImgFile = post.ImgFile;
			}

			_context.Posts.Update(existingPost);

			var postHide = await _context.PostHideByRestricted.FirstOrDefaultAsync(a => a.IdPost == idPost);

			if (postHide != null)
			{
				_context.PostHideByRestricted.Remove(postHide);
			}

			await _context.SaveChangesAsync();
			return Ok(existingPost);
		}



		[HttpPost("/deletePost/{idPost}")]
		public async Task<IActionResult> DeletePost(Guid idPost, [FromBody] Guid IdUser)
		{
			var deletePost = _context.Posts.FirstOrDefault(p => p.Id == idPost && p.IdUser == IdUser);
			if (deletePost == null)
			{
				return NotFound();
			}
			deletePost.IsDeleted = true;
			_context.Posts.Update(deletePost);
			await _context.SaveChangesAsync();
			return Ok();
		}
		private async Task<PostIntroDTO> GetPostIntro(Post post)
		{
			string Avatar;
			string UserName;

			var introPost = post.GetIntroPost();
			var groupPost = _context.GroupPosts.FirstOrDefault(a => a.IdPost == post.Id);

			var link = "";
            if (groupPost != null)
            {
                var group = await _context.Groups.FirstOrDefaultAsync(a => a.IdGroup == groupPost.IdGroup);
                UserName = group.Name;
                Avatar = group.ImgGroup==null? "/img/icon.jpg" : group.ImgGroup;
				link = "/groups/" + group.Name;
            }
            else
            {
                var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == post.IdUser);
                UserName = user.FullName;
                Avatar = user.Img;
				link = "/other-profile/" + user.Id.ToString();
            }
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

		[HttpGet("/post/getPost/{idPost}")]
		public async Task<PostIntroDTO> GetPost(Guid idPost)
		{
			var post = await _context.Posts.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == idPost && a.IsDeleted == false && a.User.LockoutEnd == null);
			if (post == null)
			{
				return null;
			}
			var postIntro = await GetPostIntro(post);
			return postIntro;
		}

	}


	// Lớp này sẽ tạo ở Data cho View dùng nữa
	public class Pagination<T>
	{
		public List<T> ListPage { get; set; }
		public int TotalPage { get; set; }
	}
}
