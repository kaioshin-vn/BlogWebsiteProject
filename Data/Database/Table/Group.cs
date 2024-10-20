using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class Group
    {
        [Key]
        public Guid IdGroup { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImgGroup { get; set; }
        public string ImgCover { get; set; }

        public KindGroup StateGroup { get; set; } // Trạng thái của nhóm 

        public ICollection<AdminGroup>? AdminGroups { get; set; }
        public ICollection<MemberGroup>? MemberGroups { get; set; }
        public ICollection<Topic>? Topics { get; set; }
    }
}
