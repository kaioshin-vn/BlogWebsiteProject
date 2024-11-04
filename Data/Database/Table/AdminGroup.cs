using BlogWebsite.Data;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class AdminGroup
    {
        public Guid IdGroup { get; set; }
        public Guid IdAdmin { get; set; }
        public Position Position { get; set; }

        [ForeignKey("IdAdmin")]
        public ApplicationUser? User { get; set; }
        [ForeignKey("IdGroup")]
        public Group? Group { get; set; }
    }
}
