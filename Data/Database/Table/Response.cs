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
    public class Response
    {
        [Key]
        public Guid Id { get; set; }

        public Guid IdUser { get; set; }

        public DateTime CreateDate { get; set; }

        public string Content { get; set; }
        public string Likes { get; set; }

        public bool IsDeleted { get; set; }
        public Guid? Mention { get; set; }

        [ForeignKey("IdUser")]
        public ApplicationUser? User { get; set; }

    }
}
