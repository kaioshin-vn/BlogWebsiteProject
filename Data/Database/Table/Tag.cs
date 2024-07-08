using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class Tag
    {
        [Key]
        public Guid Id { get; set; }

        public string TagName { get; set; }
        public ICollection<PostTag>? TagPosts { get; set; }

    }
}
