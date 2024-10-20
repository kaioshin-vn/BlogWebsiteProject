using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class Topic
    {
        [Key]
        public Guid IdTopic { get; set; }
        public string TopicName { get; set; }
        public ICollection<GroupTopic>? GroupTopics { get; set; }
    }
}
