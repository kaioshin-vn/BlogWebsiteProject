﻿using BlogWebsite.Data;
using Data.Database.Table;
using Data.DTO.EntitiDTO;
using Data.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostSaveController : ControllerBase
    {
        ApplicationDbContext _context;
        public PostSaveController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetPostSave()
        {
            return Ok(await _context.PostSaves.ToListAsync());
        }
       
        [HttpGet("getPostInSave/{idSave}")]
        public async Task<IActionResult> GetPostInSave(Guid idSave)
        {
            if (idSave == null)
            {
                return BadRequest();
            }

            var IdUser = (await _context.Saves.FirstOrDefaultAsync(a => a.Id == idSave) ).IdUser;

            var listIdHide = _context.PostHideByRestricted.Select(a => a.IdPost).ToList();
            if (IdUser != null)
            {
                var listHide = _context.PostHides.Where(a => a.IdUser == IdUser).Select(a => a.IdPost).ToList();
                if (listHide.Count != 0)
                {
                    listIdHide.AddRange(listHide);
                }
            }

            var post = await _context.PostSaves
                .Include(p => p.Post)
                .ThenInclude(p => p.GroupPost)
                .ThenInclude(a => a.Group)
                .Include(p => p.Post.User)
                .Where(a => a.IdSave == idSave && a.Post.IsDeleted == false && (a.Post.GroupPost.Count == 0 ||
            (a.Post.GroupPost.Any(b => b.IdPost == a.Post.Id && b.WaitState == WaitState.Accept && (b.Group.StateGroup == KindGroup.Public))))
            && a.Post.User.LockoutEnd == null && !listIdHide.Contains(a.Post.Id))
                .Select(p => new PostIntroDTO {
                    Id = p.Post.Id,
                    IdUser = p.Post.IdUser,
                    Title = p.Post.Title,
                    Content = p.Post.Content,
                    ImgFile = p.Post.ImgFile,
                    Avatar = p.Post.User.UserName,
                    UserName = p.Post.User.FullName,
                })
                .ToListAsync();
            if (post == null)
            {
                return NotFound("No post in this category");
            }
            return Ok(post);
        }

        [HttpGet("getFirstImageSave/{saveId}")]
        public async Task<ActionResult<List<PostDTO>>> GetFirstImageSave(Guid saveId)
        {
            var posts = await _context.PostSaves
                .Where(ps => ps.IdSave == saveId)
                .OrderBy(ps => ps.CreateDate)
                .Select(ps => new PostDTO
                {
                    ImgFile = ps.Post.ImgFile,
                }).ToListAsync();

            return Ok(posts);
        }

        [HttpPost("addPostSave/{idPost}/{idSave}")]
        public async Task<IActionResult> AddPostSave(Guid idPost, Guid idSave)
        {
            var getIdPost = await _context.Posts.FindAsync(idPost);
            if (getIdPost == null) return BadRequest();
            var getIdSave = await _context.Saves.FindAsync(idSave);
            if (getIdSave == null) return BadRequest();

            var postSave = new PostSave()
            {
                IdPost = idPost,
                IdSave = idSave,
                CreateDate = DateTime.Now
            };
            _context.PostSaves.Add(postSave);
            await _context.SaveChangesAsync();

            return Ok(postSave);
        }

        [HttpDelete("deletePostInSave/{idSave}/{idPost}")]
        public async Task<IActionResult> DeletePostInSave(Guid idSave, Guid idPost)
        {
            var getId = await _context.PostSaves.FirstOrDefaultAsync(x => x.IdSave == idSave && x.IdPost == idPost);
            if (getId == null)
            {
                return BadRequest();
            }
            _context.PostSaves.Remove(getId);
            await _context.SaveChangesAsync();
            return Ok(getId);
        }

        [HttpDelete("deleteSaveColection/{idSave}")]
        public async Task<IActionResult> DeletePost(Guid idSave)
        {
            var listDelete =  _context.PostSaves.Where(x => x.IdSave == idSave);
            _context.PostSaves.RemoveRange(listDelete);
            var postSaveColection = _context.Saves.FirstOrDefault(x => x.Id == idSave);
            _context.Saves.Remove(postSaveColection); 
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
