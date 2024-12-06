using BlogWebsite.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class PostHide
    {
        public Guid IdPost { get; set; }
        public Guid IdUser { get; set; }
        [ForeignKey("IdPost")]
        public Post? Post { get; set; }
        [ForeignKey("IdUser")]
        public ApplicationUser? User { get; set; }
    }
}
