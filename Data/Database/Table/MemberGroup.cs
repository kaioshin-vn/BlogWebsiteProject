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
    public class MemberGroup
    {
        public Guid IdGroup { get; set; }
        public Guid IdMember { get; set; }
        [ForeignKey("IdMember")]
        public ApplicationUser? User { get; set; }
        [ForeignKey("IdGroup")]
        public Group? Group { get; set; }
    }
}
