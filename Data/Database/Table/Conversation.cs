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
    public class Conversation
    {
        [Key]
        public Guid Id { get; set; }

        public Guid IdUser { get; set; }

        public Guid IdUserReceive { get; set; }
        public bool isDeleted { get; set; }

        [ForeignKey("IdUserSend")]
        public ApplicationUser? UserSend { get; set; }
        [ForeignKey("IdUserReceive")]
        public ApplicationUser? UserReceive { get; set; }
    }
}
