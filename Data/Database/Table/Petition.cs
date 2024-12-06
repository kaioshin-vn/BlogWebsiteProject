using BlogWebsite.Data;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class Petition
    {
        [Key]
        public Guid Id { get; set; }
        public Guid IdUser { get; set; }
        public string Content { get; set; }
        public WaitState State { get; set; }

        [ForeignKey("IdUser")]
        public ApplicationUser? User { get; set; }
    }
}
