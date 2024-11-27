using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO.EntitiDTO
{
    public class ReplyDTO
    {
        public Guid Id { get; set; }
        public Guid IdUser { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public string? Content { get; set; }
        public string Like { get; set; }
        public Guid? Mention { get; set; }
        public string UserNameMention { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
