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
    public class Save
    {
        [Key]
        public Guid Id { get; set; }
        public Guid IdUser { get; set; }
        public string SaveName { get; set; }
        [ForeignKey("IdUser")]
        public ApplicationUser? User { get; set; }
        public ICollection<PostSave>? PostSaves { get; set; }
    }
}
