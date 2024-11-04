using Azure;
using BlogWebsite.Data;
using Data.Database.Table;
using Microsoft.AspNetCore.Mvc;

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
            _context.PostTags.Add(postTag);
            _context.SaveChanges();
        }
    }
}
