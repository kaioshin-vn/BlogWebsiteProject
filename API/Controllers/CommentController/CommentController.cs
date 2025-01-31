﻿using Azure.Core;
using BlogWebsite.Data;
using Data.Database.Table;
using Data.DTO;
using Data.DTO.EntitiDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.CommentController
{
    public class CommentController : Controller
    {
        ApplicationDbContext _context;
        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CommentController/Details/5
        [HttpPost("/commment/getAllComments/{idPost}")]
        public List<CommentDTO> GetCommentsPost(Guid idPost, [FromBody] List<Guid> listPostExisted)
        {
            var listCmt = new List<CommentDTO>();
            var data = _context.Responses.Where(a => a.IdPost == idPost && a.IsDeleted == false && !listPostExisted.Contains(a.Id)).OrderByDescending(a => a.CreateDate).Take(20).Include(u => u.User);
            listCmt = data.Select(a => new CommentDTO
            {
                Id = a.Id,
                Avatar = a.User.Img,
                UserName = a.User.FullName,
                Content = a.Content,
                CreateDate = a.CreateDate,
                Like = a.Likes,
                IdUser = a.IdUser
            }).ToList();
            foreach (var item in listCmt)
            {
                item.ReplyCount = _context.ReplyResponses.Where(a => a.IdResponse == item.Id && a.IsDeleted == false).Count();
            }
            
            return listCmt;
        }

        [HttpPost("/commment/getAllHotComments/{idPost}")]
        public List<CommentDTO> GetHotCommentsPost(Guid idPost, [FromBody] List<Guid> listPostExisted)
        {
            var listCmt = new List<CommentDTO>();
            var data = _context.Responses.Where(a => a.IdPost == idPost && a.IsDeleted == false && !listPostExisted.Contains(a.Id)).OrderByDescending(a => a.Likes.Length).Take(20).Include(u => u.User);
            listCmt = data.Select(a => new CommentDTO
            {
                Id = a.Id,
                Avatar = a.User.Img,
                UserName = a.User.FullName,
                Content = a.Content,
                CreateDate = a.CreateDate,
                Like = a.Likes,
                IdUser = a.IdUser
            }).ToList();
            foreach (var item in listCmt)
            {
                item.ReplyCount = _context.ReplyResponses.Where(a => a.IdResponse == item.Id && a.IsDeleted == false).Count();
            }

            return listCmt;
        }

        [HttpGet("/likeCmt/{idCmt}/{idUser}/{likeState}")]
        public async Task<IActionResult> LikePost(Guid idCmt, Guid idUser, bool likeState)
        {
            var existingCmt = await _context.Responses.FirstOrDefaultAsync(p => p.Id == idCmt);
            if (existingCmt != null)
            {
                List<string> listLike;
                if (existingCmt.Likes == null || existingCmt.Likes == "")
                {
                    listLike = new List<string>();
                }
                else
                {
                    listLike = existingCmt.Likes.Split("|").ToList();
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
                existingCmt.Likes = string.Join('|', listLike);

                _context.Responses.Update(existingCmt);
                await _context.SaveChangesAsync();
                return Ok(existingCmt.Likes);
            }
            else
            {
                return NotFound();
            }
        }


        [HttpGet("/commment/getCommentDetail/{idCmt}")]
        public async Task<CommentDTO> GetCommentsDetail(Guid idCmt)
        {
            var cmtDTO = new CommentDTO();
            var cmt = await _context.Responses.FirstOrDefaultAsync(a => a.Id == idCmt && a.IsDeleted == false);
            if (cmt == null)
            {
                return null;
            }
            var infoUser = await _context.Users.FirstOrDefaultAsync(a => a.Id == cmt.IdUser);
            cmtDTO.Avatar = infoUser.Img;
            cmtDTO.UserName = infoUser.FullName;
            cmtDTO.CreateDate = cmt.CreateDate;
            cmtDTO.Content = cmt.Content;
            cmtDTO.Like = cmt.Likes;
            cmtDTO.ReplyCount = _context.ReplyResponses.Where(a => a.IdResponse == cmt.Id && a.IsDeleted == false).Count();
            cmtDTO.Id = cmt.Id;
            cmtDTO.IdUser = cmt.IdUser;
            cmtDTO.IdPost = cmt.IdPost;
            return cmtDTO;
        }

        [HttpPut("/commment/updateComment/{idCmt}")]
        public async Task UpdateCommentDetail(Guid idCmt, [FromBody] string Content)
        {
            var cmt = await _context.Responses.FirstOrDefaultAsync(a => a.Id == idCmt);
            if (cmt != null)
            {
                cmt.Content = Content;
                _context.Responses.Update(cmt);
                _context.Update(cmt);
                _context.SaveChanges();
                Ok();
            }
            else
            {
                NotFound();
            }
        }

        [HttpDelete("/commment/deleteCmt/{idCmt}/{idUser}")]
        public async Task DeleteComment(Guid idCmt, Guid idUser)
        {
            var cmt = await _context.Responses.FirstOrDefaultAsync(a => a.Id == idCmt && a.IdUser == idUser);
            if (cmt != null)
            {
                cmt.IsDeleted = true;
                _context.Responses.Update(cmt);
                _context.Update(cmt);
                _context.SaveChanges();
                Ok();
            }
            else
            {
                NotFound();
            }
        }

        // POST: CommentController/Create
        [HttpPost("/comment/addNewComment")]
        public async Task Create([FromBody] PostCommentDTO comment)
        {
            var newComment = new Response()
            {
                Id = comment.Id,
                IdPost = comment.IdPost,
                IdUser = comment.IdUser,
                Content = comment.Content,
                CreateDate = DateTime.Now,
            };

            _context.Responses.Add(newComment);
            await _context.SaveChangesAsync();
        }
    }
}
