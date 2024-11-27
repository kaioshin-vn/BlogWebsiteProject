using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO.EntitiDTO
{
    public class CommentDTO
    {
        public Guid Id { get; set; }
        public Guid IdUser { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public string? Content { get; set; }
        public string Like { get; set; }
        public int ReplyCount { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
