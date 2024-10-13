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
    public class Flower
    {
        public Guid IdUser { get; set; }

        public Guid IdFlower { get; set; }

        [ForeignKey("IdFlower")]
        public ApplicationUser? UserFlower { get; set; }
        [ForeignKey("IdUser")]
        public ApplicationUser? User { get; set; }
    }
}
