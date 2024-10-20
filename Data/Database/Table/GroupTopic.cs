using BlogWebsite.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class GroupTopic
    {
        public Guid IdGroup { get; set; }
        public Guid IdTopic { get; set; }

        [ForeignKey("IdTopic")]
        public Topic? Topic { get; set; }
        [ForeignKey("IdGroup")]
        public Group? Group { get; set; }
    }
}
