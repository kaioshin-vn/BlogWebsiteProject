﻿using API.StaticClass;
using Azure;
using Blazorise;
using BlogWebsite.Data;
using Data.Database.Table;
using Data.DTO;
using Data.DTO.EntitiDTO;
using Data.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.TagController
{
    public class TagController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TagController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("/addNewTag")]
        public string AddTag([FromBody]Tag tag)
        {
            if (!_context.Tags.Any(a => a.TagName == tag.TagName))
            {
                _context.Tags.Add(tag);
                _context.SaveChanges();
                return tag.Id.ToString();
            }
            else
            {
                return _context.Tags.FirstOrDefault(a => a.TagName == tag.TagName).Id.ToString();
            }
        }

        [HttpPost("/addNewPostTag")]
        public void AddPostTag([FromBody]PostTag postTag)
        {
            if ( !_context.PostTags.Any(a => a.IdPost == postTag.IdPost && a.IdTag == postTag.IdTag))
            {
                _context.PostTags.Add(postTag);
                _context.SaveChanges();
            }
            
        }

        [HttpGet("/AllPostTag/{IdUser?}/{IdTag}")]
        public async Task<IActionResult> AllGroupTopic(Guid? IdUser, Guid IdTag)
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

            var tag = new TagDTO();

            if (_context.PostTags.Count() != 0)
            {
                var tp = await _context.Tags.FirstOrDefaultAsync(a => a.Id == IdTag);
                tag.Id = tp.Id;
                tag.Name = tp.TagName;
            }

            var listIdPost = new List<Guid>();
            if (_context.PostTags.Count() != 0)
            {
                listIdPost = _context.PostTags.Where(a => a.IdTag == tag.Id).Select(a => a.IdPost).ToList();
            }

            var listPostIntro = new List<PostIntroDTO>();

            var listPost = _context.Posts.Include(a => a.User).Include(a => a.GroupPost).ThenInclude(a => a.Group).Where(a => a.IsDeleted == false && (a.GroupPost.Count == 0 ||
            (a.GroupPost.Any(b => b.IdPost == a.Id && b.WaitState == WaitState.Accept && (b.Group.StateGroup == KindGroup.Public))))
            && a.User.LockoutEnd == null && !listIdHide.Contains(a.Id) && listIdPost.Contains(a.Id)).ToList();

            foreach (var item in listPost)
            {
                var postIntro = await GetPostIntro(item);
                listPostIntro.Add(postIntro);
            }

            tag.PostDTOs = listPostIntro;

            return Ok(tag);
        }


        [HttpGet("/SearchAllPostTag/{IdUser?}/{TopicName}")]
        public async Task<IActionResult> SearchAllGroupTopic(Guid? IdUser, string TopicName)
        {
            var listIdHide = new List<Guid>();
            if (IdUser != null)
            {
                var listHide = _context.PostHides.Where(a => a.IdUser == IdUser).Select(a => a.IdPost).ToList();
                if (listHide.Count != 0)
                {
                    listIdHide = listHide;
                }
            }
            var tag = new TagDTO();

            if (_context.GroupTopics.Count() != 0)
            {
                var tp = await _context.Tags.FirstOrDefaultAsync(a => a.TagName.ToLower().Contains((TopicName.ToLower())));
                tag.Id = tp.Id;
                tag.Name = tp.TagName;
            }

            var listIdPost = new List<Guid>();
            if (_context.PostTags.Count() != 0)
            {
                listIdPost = _context.PostTags.Where(a => a.IdTag == tag.Id).Select(a => a.IdPost).ToList();
            }

            var listPostIntro = new List<PostIntroDTO>();

            var listPost = _context.Posts.Include(a => a.User).Include(a => a.GroupPost).ThenInclude(a => a.Group).Where(a => a.IsDeleted == false && (a.GroupPost.Count == 0 ||
            (a.GroupPost.Any(b => b.IdPost == a.Id && b.WaitState == WaitState.Accept && (b.Group.StateGroup == KindGroup.Public))))
            && a.User.LockoutEnd == null && !listIdHide.Contains(a.Id) && listIdPost.Contains(a.Id)).ToList();

            foreach (var item in listPost)
            {
                var postIntro = await GetPostIntro(item);
                listPostIntro.Add(postIntro);
            }

            tag.PostDTOs = listPostIntro;

            return Ok(tag);
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
