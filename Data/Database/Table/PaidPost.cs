using BlogWebsite.Data;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class PaidPost
    {
        public Guid Id { get; set; }

        public Guid IdPost { get; set; }
        public Guid IdUser { get; set; }

        public int AmountPay { get; set; }
        public int ViewCount {get;set;}

        [ForeignKey("IdPost")]
        public Post? Post { get; set; }
        [ForeignKey("IdUser")]
        public ApplicationUser? User { get; set; }
    }
}
