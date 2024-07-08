using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class PostSave
    {
        public Guid IdSave { get; set; }
        public Guid IdPost { get; set; }
        [ForeignKey("IdSave")]
        public Save? Save { get; set; }
        [ForeignKey("IdPost")]
        public Post? Post { get; set; }
    }
}
