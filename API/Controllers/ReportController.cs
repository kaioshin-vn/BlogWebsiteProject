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
using Data.Enums;

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
            var listPostRepost = _context.Reports.Include(a => a.Post).ThenInclude(a => a.User).Where(a => a.State == Data.Enums.WaitState.Pending && a.Post.IsDeleted == false && a.Post.User.LockoutEnd == null).GroupBy(a => a.IdPost).ToList().Skip(page * 10).Take(10).ToList();

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

        [HttpGet("/getListUserReport/{IdUser}")]
        public async Task<IActionResult> GetReportUser(Guid IdUser)
        {
            var listPostRepost = _context.Reports.Include(a => a.Post).ThenInclude(a => a.User).Where(a => a.State == Data.Enums.WaitState.Accept && a.IdUser == IdUser && a.Post.User.LockoutEnd == null).GroupBy(a => a.IdPost).ToList();

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
            var post = await _context.Posts.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == idPost && a.IsDeleted == false && a.User.LockoutEnd == null);
            if (post == null)
            {
                return null;
            }
            var postIntro = await GetPostIntro(post);
            return postIntro;
        }

        [HttpGet("/report/getReportedPost/{idPost}")]
        public async Task<PostIntroDTO> GetPostReported(Guid idPost)
        {
            var post = await _context.Posts.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == idPost && a.User.LockoutEnd == null);
            if (post == null)
            {
                return null;
            }
            var postIntro = await GetPostIntro(post);
            return postIntro;
        }

        [HttpGet("/report/changeStateReport/{idPost}/{State}")]
        public async Task ChangeStateReport(Guid idPost, WaitState State)
        {
            var reports = _context.Reports.Where(a => a.IdPost == idPost && a.State == WaitState.Pending).ToList();

            reports.ForEach(a => a.State = State);

            _context.Reports.UpdateRange(reports);
            await _context.SaveChangesAsync();
            Ok();
        }

        [HttpGet("/report/getReporDetail/{idPost}")]
        public async Task<List<UserReport>> GetDetailPost(Guid idPost)
        {
            var reposts = _context.Reports.Include(a => a.UserReport).Where(a => a.State == Data.Enums.WaitState.Pending && a.IdPost == idPost).ToList();

            var listRepost = new List<UserReport>();
            foreach (var item in reposts)
            {
                var userRp = new UserReport();
                userRp.Img = item.UserReport.Img;
                userRp.UserName = item.UserReport.FullName;
                userRp.Id = item.UserReport.Id;
                userRp.Content = item.ContentReport;
                listRepost.Add(userRp);
            }

            return listRepost;
        }

        [HttpGet("/report/getReportedDetail/{idPost}")]
        public async Task<List<UserReport>> GetReportedDetailPost(Guid idPost)
        {
            var reposts = _context.Reports.Include(a => a.UserReport).Where(a => a.State == Data.Enums.WaitState.Accept && a.IdPost == idPost).ToList();

            var listRepost = new List<UserReport>();
            foreach (var item in reposts)
            {
                var userRp = new UserReport();
                userRp.Img = item.UserReport.Img;
                userRp.UserName = item.UserReport.FullName;
                userRp.Id = item.UserReport.Id;
                userRp.Content = item.ContentReport;
                listRepost.Add(userRp);
            }

            return listRepost;
        }

        [HttpGet("/getTotalPageReport")]
        public async Task<int> GetTotalPage()
        {
            return _context.Reports.Include(a => a.Post).Where(a => a.State == Data.Enums.WaitState.Pending && a.Post.IsDeleted == false).GroupBy(a => a.IdPost).Count() / 10;
        }

        [HttpGet("/getViolation/{Id}")]
        public async Task<string> GetViolation(Guid Id)
        {
            var notice = await _context.Notices.FirstOrDefaultAsync(a => a.Id == Id);
            return notice.Content;
        }

        [HttpDelete("/deletereport/{IdPost}/{IdUserReport}/{IdUser}")]
        public async Task DeleteReport(Guid IdPost, Guid IdUserReport, Guid IdUser)
        {
            var rp = await _context.Reports.FirstOrDefaultAsync(a => a.IdPost == IdPost && a.IdUser == a.IdUser && a.IdUserReport == IdUserReport);
            _context.Reports.Remove(rp);
            await _context.SaveChangesAsync();
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
