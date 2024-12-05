using API.StaticClass;
using Azure;
using Blazorise;
using BlogWebsite.Data;
using Data.Database.Table;
using Data.DTO;
using Data.DTO.EntitiDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.TagController
{
    public class Statistroller : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public Statistroller(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("/getstatisUser/{year}")]
        public List<int> GetStatisUser(int year)
        {
            var listUser = _context.Users.Where(a => a.CreateTime.Value.Year == year).ToList();

            var listMonth = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var listTotalUser = new List<int>() ;

            foreach (var item in listMonth)
            {
                listTotalUser.Add(listUser.Where(a => a.CreateTime.Value.Month == item).Count());
            }          
            return listTotalUser;
        }

        [HttpGet("/getStatisTotal")]
        public Dictionary<string,string> getStatisTotal()
        {
            var total = new Dictionary<string,string>();
            total["Account"] = _context.Users.Count().ToString();
            total["Post"] = _context.Posts.Count(a => a.IsDeleted == false).ToString();
            total["Group"] = _context.Groups.Count(a => a.isDeleted == false).ToString();
            return total;
        }

        [HttpGet("/getstatisGroup/{year}")]
        public List<int> GetStatisGroup(int year)
        {
            var listUser = _context.Groups.Where(a => a.DateTime.Year == year && a.isDeleted == false).ToList();

            var listMonth = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var listTotalUser = new List<int>();

            foreach (var item in listMonth)
            {
                listTotalUser.Add(listUser.Where(a => a.DateTime.Month == item).Count());
            }
            return listTotalUser;
        }

        [HttpGet("/getstatisPost/{year}")]
        public List<int> GetStatisPost(int year)
        {
            var listUser = _context.Posts.Where(a => a.CreateDate.Value.Year == year && a.IsDeleted == false).ToList();

            var listMonth = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var listTotalUser = new List<int>();

            foreach (var item in listMonth)
            {
                listTotalUser.Add(listUser.Where(a => a.CreateDate.Value.Month == item).Count());
            }
            return listTotalUser;
        }

    }
}
