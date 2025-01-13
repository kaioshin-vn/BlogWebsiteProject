using API.StaticClass;
using Blazorise.Utilities;
using BlogWebsite.Data;
using Data.Database.Table;
using Data.DTO;
using Data.DTO.EntitiDTO;
using Data.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("getListPostTop/{idUser}")]
        public async Task<List<PostIntroDTO>> GetListPostTop(Guid idUser)
        {
            var listIdHide = _context.PostHideByRestricted.Select(a => a.IdPost).ToList();
            if (idUser != null)
            {
                var listHide = _context.PostHides.Where(a => a.IdUser == idUser).Select(a => a.IdPost).ToList();
                if (listHide.Count != 0)
                {
                    listIdHide.AddRange(listHide);
                }
            }

            var lstPost = await _context.Posts.Include(a => a.User)
                .Include(a => a.GroupPost).ThenInclude(a => a.Group).Where(a => a.IsDeleted == false && a.User.LockoutEnd == null && !listIdHide.Contains(a.Id)
                && (a.GroupPost.Count == 0 || (a.GroupPost.Any(b => b.IdPost == a.Id && b.WaitState == WaitState.Accept && (b.Group.StateGroup == KindGroup.Public)))))
                .ToListAsync();

            var lstPostUser = new List<PostIntroDTO>();
            foreach (var item in lstPost)
            {
                var introPostUser = await GetPostIntro(item);
                lstPostUser.Add(introPostUser);
            }
            var lstPostTop = lstPostUser.OrderByDescending(x => x.Like.Length).Take(10).ToList();
            return lstPostTop;
        }

        [HttpGet("SearchListPostIntro/{idUser}")]
        public async Task<List<PostIntroDTO>> GetListPostIntro(Guid idUser)
        {
            var listIdHide = _context.PostHideByRestricted.Select(a => a.IdPost).ToList();
            if (idUser != null)
            {
                var listHide = _context.PostHides.Where(a => a.IdUser == idUser).Select(a => a.IdPost).ToList();
                if (listHide.Count != 0)
                {
                    listIdHide.AddRange(listHide);
                }
            }

            var listPost = await _context.Posts.Include(a => a.User)
                .Include(a => a.GroupPost).ThenInclude(a => a.Group).Where(a => a.IsDeleted == false && a.User.LockoutEnd == null && !listIdHide.Contains(a.Id)
                && (a.GroupPost.Count == 0 || (a.GroupPost.Any(b => b.IdPost == a.Id && b.WaitState == WaitState.Accept && (b.Group.StateGroup == KindGroup.Public)))))
                .ToListAsync();




            var listIntroPost = new List<PostIntroDTO>();
            foreach (var item in listPost)
            {
                var introPost = await GetPostIntro(item);
                listIntroPost.Add(introPost);
            }
            return listIntroPost;
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
                Avatar = group.ImgGroup == null ? "/img/icon.jpg" : group.ImgGroup;
                link = "/groups/" + group.Name;
            }
            else
            {
                var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == post.IdUser);
                UserName = user.FullName;
                Avatar = user.Img;
                link = "/other-profile/" + user.Id.ToString();
            }
            var commment = _context.Responses.Where(a => a.IdPost == post.Id);
            var replyCount = 0;
            var replyComment = _context.ReplyResponses.Where(a => commment.Any(b => b.Id == a.IdResponse));

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

        [HttpGet("search-all")]
        public async Task<ActionResult<SearchResultWithPaginationDTO>> SearchAll(Guid idUser, string keyword, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Ok(new SearchResultWithPaginationDTO());

            var listIdHide = _context.PostHideByRestricted.Select(a => a.IdPost).ToList();


            // Lưu keyword search
            if (idUser != Guid.Empty)
            {
                await SaveSearchHistory(keyword, idUser);
                var listHide = _context.PostHides.Where(a => a.IdUser == idUser).Select(a => a.IdPost).ToList();
                if (listHide.Count != 0)
                {
                    listIdHide.AddRange(listHide);
                }
            }

            // Chuẩn hóa từ khóa tìm kiếm
            var normalizedKeyword = keyword.ToLower().RemoveDiacritics();
            var originalKeyword = keyword.ToLower();



            // POSTS
            var posts = await _context.Posts.Include(a => a.User)
                .Include(a => a.GroupPost).ThenInclude(a => a.Group).Where(a => a.IsDeleted == false && a.User.LockoutEnd == null && !listIdHide.Contains(a.Id)
                && (a.GroupPost.Count == 0 || (a.GroupPost.Any(b => b.IdPost == a.Id && b.WaitState == WaitState.Accept && (b.Group.StateGroup == KindGroup.Public )))))
                .ToListAsync();

            // listPost tìm được qua keyword, lấy những title gần nhất với keyword
            var allFilteredPosts = posts
                .Where(p =>
                    (p.Title != null && (
                        p.Title.ToLower().RemoveDiacritics().Contains(normalizedKeyword) ||
                        p.Title.ToLower().Contains(originalKeyword)
                    )))
                .OrderByDescending(p => p.Title != null &&
                    (p.Title.ToLower().StartsWith(normalizedKeyword) ||
                     p.Title.ToLower().StartsWith(originalKeyword)))
                .ToList();
            // Count Post tìm được
            var totalPosts = allFilteredPosts.Count;
            // Pagination
            var pagedPosts = allFilteredPosts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var postDtos = new List<PostIntroDTO>();
            foreach (var post in pagedPosts)
            {
                var postDto = await GetPostIntro(post);
                postDtos.Add(postDto);
            }

            // USERS
            var users = await _context.Users.ToListAsync();

            var allFilteredUsers = users
                .Where(u =>
                    u.FullName.ToLower().RemoveDiacritics().Contains(normalizedKeyword) ||
                    u.FullName.ToLower().Contains(originalKeyword))
                 .OrderByDescending(p => p.FullName != null &&
                    (p.FullName.ToLower().StartsWith(normalizedKeyword) ||
                     p.FullName.ToLower().StartsWith(originalKeyword)))
                .ToList();

            var totalUsers = allFilteredUsers.Count;

            var pagedUsers = allFilteredUsers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserDto
                {
                    Id = u.Id.ToString(),
                    FullName = u.FullName,
                    Img = u.Img,
                    Address = u.Address,
                    Email = u.Email,
                    CountFollow = _context.Flower.Where(c => c.IdUser == u.Id && c.IsFollowing == true).Count(),
                    Descript = u.Descript,
                })
                .ToList();

            // GROUPS
            var groups = await _context.Groups.ToListAsync();

            var allFilteredGroups = groups
                .Where(g =>
                    g.Name.ToLower().RemoveDiacritics().Contains(normalizedKeyword) ||
                    g.Name.ToLower().Contains(originalKeyword))
                .OrderByDescending(p => p.Name != null &&
                    (p.Name.ToLower().StartsWith(normalizedKeyword) ||
                     p.Name.ToLower().StartsWith(originalKeyword)))
                .ToList();

            var totalGroups = allFilteredGroups.Count;

            var pagedGroups = allFilteredGroups
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(g => new GroupDTO
                {
                    IdGroup = g.IdGroup,
                    Name = g.Name,
                    Description = g.Description,
                    ImgGroup = g.ImgGroup == null ? "/img/icon.jpg" : g.ImgGroup,
                    StateGroup = g.StateGroup,
                    MemberCount = _context.MemberGroups.Count(m => m.IdGroup == g.IdGroup)
                })
                .ToList();

            return Ok(new SearchResultWithPaginationDTO
            {
                Posts = postDtos,
                Users = pagedUsers,
                Groups = pagedGroups,
                TotalPosts = totalPosts,
                TotalUsers = totalUsers,
                TotalGroups = totalGroups,
                CurrentPage = page,
                PageSize = pageSize
            });
        }

        private async Task SaveSearchHistory(string keyword, Guid idUser)
        {
            try
            {
                var searchHistory = new SearchHistory
                {
                    Id = Guid.NewGuid(),
                    IdUser = idUser,
                    Keyword = keyword,
                    SearchDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)
                };

                _context.SearchHistories.Add(searchHistory);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                Console.WriteLine($"Error saving search history: {ex.Message}");
            }
        }

        [HttpGet("get-search-history")]
        public async Task<ActionResult<List<string>>> GetSearchHistory(Guid idUser, int take = 5)
        {
            var history = await _context.SearchHistories
                .Where(x => x.IdUser == idUser)
                .OrderByDescending(x => x.SearchDate)
                .Select(x => x.Keyword)
                .Distinct()
                .Take(take)
                .ToListAsync();

            return Ok(history);
        }

        [HttpDelete("remove-history-search")]
        public async Task<IActionResult> DeleteHistorySearch(Guid idUser, string keyword)
        {
            var hisrytoSearch = await _context.SearchHistories.Where(x => x.Keyword == keyword && x.IdUser == idUser).ToListAsync();
            if(hisrytoSearch == null)
            {
                return BadRequest();
            }
            foreach (var item in hisrytoSearch)
            {
                _context.SearchHistories.Remove(item);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
    // Thêm extension method để xử lý dấu
    public static class StringExtensions
    {
        public static string RemoveDiacritics(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
