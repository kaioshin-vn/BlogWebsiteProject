using BlogWebsite.Data;
using Data.Database.Table;
using Data.DTO;
using Data.DTO.EntitiDTO;
using Microsoft.AspNetCore.Mvc;

namespace API.StaticClass
{
    public static class ExtendsionMethod
    {
        public static Pagination<T> GetObject<T>(this List<T> listValue, int page, int pagesize = 10) where T : class
        {
            if (page <= 0)
            {
                page = 1;
            }

            var totalPage = listValue.Count / pagesize + 1;

            if (page > totalPage)
            {
                page = totalPage;
            }

            return new Pagination<T>()
            {
                TotalPage = totalPage,
                ListPage = listValue.Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList()
            };
        }

        public static PostIntroDTO GetIntroPost(this Post post)
        {
            var introPost = new PostIntroDTO() 
            {
                Id = post.Id,  
                IdUser = post.IdUser,
                Title = post.Title,
                ImgFile = post.ImgFile,
                VideoFile = post.VideoFile,
                Like = post.Like,
                Content = post.Content,
                CreateDate = post.CreateDate,
            };
            return introPost;
        }
    }
}
