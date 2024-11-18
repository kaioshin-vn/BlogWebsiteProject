using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO.EntitiDTO
{
    public class PostIntroDTO
    {
        public Guid Id { get; set; }

        public string UserName { get; set; } 

        public string Avatar { get; set; }
        public string Title { get; set; }
        public string? Content { get; set; }
        public string? VideoFile { get; set; }
        public string? ImgFile { get; set; }
        public string Like { get; set; }
        public int CommentCount { get; set; }
        public DateTime? CreateDate { get; set; }
        public List<string>? ListTag { get; set; }
    }
}
