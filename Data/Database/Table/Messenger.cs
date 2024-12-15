using BlogWebsite.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class Messenger
    {
        [Key]
        public Guid Id { get; set; }
        public Guid IdUserSend { get; set; }
        public Guid? IdUserReceive { get; set; }

        public Guid IdConversation { get;set; }
        public Guid? IdReply { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("IdUserSend")]
        public ApplicationUser? UserSend { get; set; } 
        [ForeignKey("IdUserReceive")]
        public ApplicationUser? UserReceive { get; set; }

        [ForeignKey("IdConversation")]
        public Conversation? Conversation { get; set; }
    }
}
