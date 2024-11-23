using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO.EntitiDTO
{
    public class PostCommentDTO
    {
        public Guid Id { get; set; }
        public Guid IdUser { get; set; }
        public Guid IdPost { get; set; }
        public string Content { get; set; }
        public string Likes { get; set; } = "";
        public DateTime CreateDate { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
