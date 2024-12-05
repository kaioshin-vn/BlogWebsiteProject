using BlogWebsite.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Enums;

namespace Data.Database.Table
{
    public class Report
    {
        [Key]
        public Guid Id { get; set; }

        public Guid IdUserReport { get; set; }
        public Guid IdUser { get; set; }
        public Guid IdPost { get; set; }

        public string ContentReport { get; set; }

        public WaitState State {  get; set; }

        [ForeignKey("IdUserReport")]
        public ApplicationUser? UserReport { get; set; }

        [ForeignKey("IdPost")]
        public Post? Post { get; set; }

        [ForeignKey("IdUser")]
        public ApplicationUser? User { get; set; }
    }
}
