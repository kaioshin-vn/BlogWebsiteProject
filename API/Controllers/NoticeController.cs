﻿using BlogWebsite.Data;
using Data.Database.Table;
using Data.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Controllers
{
    [ApiController]
    public class NoticeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NoticeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("getAmountNoticeUser/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var noticeCount = _context.Notices.Where(a => a.ToUser == id && a.isSeen == false).Count();
            return Ok(noticeCount);
        }

        [HttpGet("getAllNoticeUser/{id}/{Next}")]
        public async Task<List<NoticeDTO>> GetAllById(Guid id, int Next)
        {
            var notices = _context.Notices.Where(a => a.ToUser == id).OrderByDescending(a => a.CreateDate).Skip(Next).Take(6).Include(a => a.UserSend).ToList();
            if (notices != null)
            {
                return notices.Select(a =>
                    new NoticeDTO
                    {
                        Id = a.Id,
                        IdUserSend = a.FromUser,
                        AvatarUserSend = a.Link.Contains("violation") || a.FromUser == a.ToUser ? @"\Img\avatar_default1.jpg" : a.UserSend.Img,
                        UserNameSend = a.Link.Contains("violation") || a.FromUser == a.ToUser ? "Admin" : a.UserSend.FullName,
                        Link = a.Link,
                        Content = a.Content,
                        isRead = a.isRead,
                        Time = ProcessTime(a.CreateDate)
                    }
                 ).ToList();
            }
            return new List<NoticeDTO>();
        }

        private string ProcessTime(DateTime? CreateDate)
        {
            string Date;

            var timeCacul = DateTime.Now - CreateDate.Value;

            int yearsDifference = DateTime.Now.Year - CreateDate.Value.Year;
            if (DateTime.Now < CreateDate.Value.AddYears(yearsDifference))
            {
                yearsDifference--;
            }

            int monthsDifference = (DateTime.Now.Year - CreateDate.Value.Year) * 12 + DateTime.Now.Month - CreateDate.Value.Month;
            if (DateTime.Now.Day < CreateDate.Value.Day)
            {
                monthsDifference--;
            }

            if (yearsDifference >= 1)
            {
                Date = $"{yearsDifference} năm";
            }
            else if (monthsDifference >= 1)
            {
                Date = $"{monthsDifference} tháng";
            }
            else if (timeCacul.TotalDays >= 1)
            {
                Date = $"{(int)timeCacul.TotalDays} ngày";
            }
            else if (timeCacul.TotalHours >= 1)
            {
                Date = $"{(int)timeCacul.TotalHours} giờ";
            }
            else if (timeCacul.TotalMinutes >= 1)
            {
                Date = $"{(int)timeCacul.TotalMinutes} phút";
            }
            else
            {
                Date = "Vừa xong";
            }
            return Date;
        }

        [HttpPost("/addNewNotice")]
        public async Task AddNewNotice([FromBody] Notice Notice)
        {
            _context.Notices.Add(Notice);
            await _context.SaveChangesAsync();
        }

        [HttpPost("/addNewPostNoticeUser")]
        public async Task<bool> AddNewNoticePost([FromBody] Notice Notice)
        {
            var noticeExisted = _context.Notices.FirstOrDefault(a => a.Link == Notice.Link && a.Content == "Đã bình luận về bài viết của bạn");
            if (noticeExisted != null)
            {
                if ((DateTime.Now - noticeExisted.CreateDate).TotalMinutes >= 5)
                {
                    noticeExisted.FromUser = Notice.FromUser;
                    noticeExisted.isSeen = false;
                    noticeExisted.isRead = false;
                    noticeExisted.CreateDate = Notice.CreateDate;
                    _context.Notices.Update(noticeExisted);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                _context.Notices.Add(Notice);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        [HttpGet("/getIdUserPost/{IdPost}")]
        public async Task<string> getIdUserPost(Guid IdPost)
        {
            var post = _context.Posts.FirstOrDefault(a => a.Id == IdPost);
            return post.IdUser.ToString();
        }

        [HttpGet("/ClearNotice/{IdUser}")]
        public async Task ClearNotice(Guid IdUser)
        {
            var listId = new List<Guid>();
            var result = _context.Notices.Where(a => a.ToUser == IdUser).ToList();

            foreach (var item in result)
            {
                item.isSeen = true;
            }

            _context.UpdateRange(result);
            await _context.SaveChangesAsync();
        }

        [HttpGet("/ReadedNotice/{Id}")]
        public async Task ReadedNotice(Guid Id)
        {
            var listId = new List<Guid>();
            var result = await _context.Notices.FirstOrDefaultAsync(a => a.Id == Id);

            if (result != null)
            {
                result.isRead = true;
                _context.UpdateRange(result);
                await _context.SaveChangesAsync();
            }
        }

    }
}
