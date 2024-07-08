using BlogWebsite.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class Notice
    {
        [Key]
        public Guid Id { get; set; }

        public Guid FromUser { get; set; }
        public Guid ToUser { get; set; }

        public DateTime CreateDate { get; set; }

        public string Content { get; set; }

        public bool isRead { get; set; }

        public string Link { get; set; }

        public bool isSeen { get; set; }

        [ForeignKey("ToUser")]
        public ApplicationUser? UserReceive { get; set; }

        [ForeignKey("FromUser")]
        public ApplicationUser? UserSend { get; set; }
    }
}
