using API.StaticClass;
using BlogWebsite.Data;
using Data.Database;
using Data.Database.Table;
using Data.DTO.EntitiDTO;
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

        [HttpGet("/GetListPostIntro/{idUser}")]
        public async Task<List<PostIntroDTO>> GetListPostIntro(Guid idUser)
        {
            var listPost = await _context.Posts.Where(a => a.IsDeleted == false).ToListAsync();

            var listIntroPost = new List<PostIntroDTO>();
            foreach (var item in listPost)
            {
                var introPost = await GetPostIntro(item);
                listIntroPost.Add(introPost);
            }
            return listIntroPost;
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

            if (groupPost != null)
            {
                var group = await _context.Groups.FirstOrDefaultAsync(a => a.IdGroup == groupPost.IdGroup);
                UserName = group.Name;
                Avatar = group.ImgGroup;
            }
            else
            {
                var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == post.IdUser);
                UserName = user.FullName;
                Avatar = user.Img;
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
            return introPost;
        }

        [HttpGet("/post/getPost/{idPost}")]
        public async Task<PostIntroDTO> GetPost(Guid idPost)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(a => a.Id == idPost);
            if (post == null)
            {
                NotFound();
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
