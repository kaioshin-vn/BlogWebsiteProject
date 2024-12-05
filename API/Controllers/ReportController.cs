using Microsoft.AspNetCore.Http;
using BlogWebsite.Data;
using Data.Database.Table;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data.DTO.EntitiDTO;
using Data.DTO;
using Blazorise;
using Client.Components.Pages.Restricted;
using API.StaticClass;

namespace API.Controllers
{
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("/addReport")]
        public async Task<IActionResult> GetAllTopic([FromBody] Report report)
        {
            await _context.Reports.AddAsync(report);
            _context.SaveChanges();
            return Ok();
        }
        [HttpGet("/getListReport/{page}")]
        public async Task<IActionResult> GetTopics(int page)
        {
            page = page - 1;
            var listPostRepost = _context.Reports.Include(a => a.Post).Where(a => a.State == Data.Enums.WaitState.Pending && a.Post.IsDeleted == false).GroupBy(a => a.IdPost).ToList().Skip(page * 10).Take(10).ToList();

            var listReport = new List<ReportDTO>();
            foreach (var item in listPostRepost)
            {
                var report = new ReportDTO();
                report.IdPost = item.Key;
                var post = await _context.Posts.FirstOrDefaultAsync(a => a.Id == item.Key);
                report.PostIntro = await GetPostIntro(post);
                report.ReportCount = item.Count();

                listReport.Add(report);
            }
            return Ok(listReport);
        }

        [HttpGet("/report/getReportPost/{idPost}")]
        public async Task<PostIntroDTO> GetPost(Guid idPost)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(a => a.Id == idPost && a.IsDeleted == false);
            if (post == null)
            {
                return null;
            }
            var postIntro = await GetPostIntro(post);
            return postIntro;
        }

        [HttpGet("/getTotalPageReport")]
        public async Task<int> GetTotalPage()
        {
            return _context.Reports.Include(a => a.Post).Where(a => a.State == Data.Enums.WaitState.Pending && a.Post.IsDeleted == false).GroupBy(a => a.IdPost).Count() / 10;
        }


        private async Task<PostIntroDTO> GetPostIntro(Post post)
        {
            string Avatar;
            string UserName;

            var introPost = post.GetIntroPost();
            var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == post.IdUser);
            UserName = user.FullName;
            Avatar = user.Img;
            var link = "/other-profile/" + user.Id.ToString();
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
    }
}
