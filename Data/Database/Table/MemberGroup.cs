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
    public class MemberGroup
    {
        public Guid IdGroup { get; set; }
        public Guid IdMember { get; set; }
        public Position Position { get; set; }

		[ForeignKey(nameof(IdMember))]
		public ApplicationUser? User { get; set; }

		[ForeignKey(nameof(IdGroup))]
		public Group? Group { get; set; }
    }
}
