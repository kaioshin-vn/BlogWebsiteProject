using BlogWebsite.Data;
using Data.Database.Table;
using Data.DTO.EntitiDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.ReplyCommentController
{
    public class ReplyCommentController : Controller
    {
        ApplicationDbContext _context;
        public ReplyCommentController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("/comment/addNewReplyComment")]
        public async Task Create([FromBody] ReplyResponse replyComment)
        {
            replyComment.CreateDate = DateTime.Now;
            _context.ReplyResponses.Add(replyComment);
            await _context.SaveChangesAsync();
        }

        [HttpPost("/commment/getAllReplyComments/{IdCmt}")]
        public List<ReplyDTO> GetCommentsPost(Guid IdCmt, [FromBody] List<Guid> listPostExisted)
        {
            var listCmt = new List<ReplyDTO>();
            var data = _context.ReplyResponses.Where(a => a.IdResponse == IdCmt && a.IsDeleted == false && !listPostExisted.Contains(a.Id)).OrderBy(a => a.CreateDate).Take(20).Include(u => u.User);
            listCmt = data.Select(a => new ReplyDTO
            {
                Id = a.Id,
                Avatar = a.User.Img,
                UserName = a.User.FullName,
                Content = a.Content,
                CreateDate = a.CreateDate,
                Like = a.Likes,
                IdUser = a.IdUser,
                Mention = a.Mention,
            }).ToList();
            return listCmt;
        }

        [HttpGet("/likeReplyCmt/{idReply}/{idUser}/{likeState}")]
        public async Task<IActionResult> LikePost(Guid idReply, Guid idUser, bool likeState)
        {
            var existingCmt = await _context.ReplyResponses.FirstOrDefaultAsync(p => p.Id == idReply);
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

                _context.ReplyResponses.Update(existingCmt);
                await _context.SaveChangesAsync();
                return Ok(existingCmt.Likes);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("/commment/getReplyCommentDetail/{idReplyCmt}")]
        public async Task<ReplyDTO> GetCommentsDetail(Guid idReplyCmt)
        {
            var cmtDTO = new ReplyDTO();
            var cmt = await _context.ReplyResponses.FirstOrDefaultAsync(a => a.Id == idReplyCmt && a.IsDeleted == false);
            var infoUser = await _context.Users.FirstOrDefaultAsync(a => a.Id == cmt.IdUser);
            cmtDTO.Avatar = infoUser.Img;
            cmtDTO.UserName = infoUser.FullName;
            cmtDTO.CreateDate = cmt.CreateDate;
            cmtDTO.Content = cmt.Content;
            cmtDTO.Like = cmt.Likes;
            cmtDTO.Id = cmt.Id;
            cmtDTO.IdUser = cmt.IdUser;
            return cmtDTO;
        }

        [HttpPut("/commment/updateReplyComment/{idCmt}")]
        public async Task UpdateCommentDetail(Guid idCmt, [FromBody] string Content)
        {
            var cmt = await _context.ReplyResponses.FirstOrDefaultAsync(a => a.Id == idCmt);
            if (cmt != null)
            {
                cmt.Content = Content;
                _context.ReplyResponses.Update(cmt);
                _context.Update(cmt);
                _context.SaveChanges();
                Ok();
            }
            else
            {
                NotFound();
            }
        }

        [HttpDelete("/commment/deleteReplyCmt/{idCmt}/{idUser}")]
        public async Task DeleteComment(Guid idCmt, Guid idUser)
        {
            var cmt = await _context.ReplyResponses.FirstOrDefaultAsync(a => a.Id == idCmt && a.IdUser == idUser);
            if (cmt != null)
            {
                cmt.IsDeleted = true;
                _context.ReplyResponses.Update(cmt);
                _context.Update(cmt);
                _context.SaveChanges();
                Ok();
            }
            else
            {
                NotFound();
            }
        }

    }
}
